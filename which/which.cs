using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using GSharpTools;

namespace which
{
    /// <summary>
    /// Find executable files on the PATH environment 
    /// </summary>
    class Which
    {
        /// <summary>
        /// Helper class for parsing input arguments
        /// </summary>
        private InputArgs Args;

        /// <summary>
        /// List of filenames to locate
        /// </summary>
        private List<string> Filenames;

        /// <summary>
        /// List of directories to locate files in
        /// </summary>
        private List<string> Directories;

        /// <summary>
        /// This program finds an executable on the PATH. It can also find other stuff on the path, but 
        /// mostly it finds the executable.s
        /// </summary>
        /// <param name="args"></param>
        private void Run(string[] args)
        {
            Console.OutputEncoding = Encoding.GetEncoding(Encoding.Default.CodePage);

            Args = new InputArgs( "which", string.Format( resource.IDS_TITLE, AppVersion.Get() ) + "\r\n" + resource.IDS_COPYRIGHT );


            Args.Add(InputArgType.StringList, "extension", null, Presence.Optional, resource.IDS_CMD_extension_doc);
            Args.Add(InputArgType.StringList, "dir", null, Presence.Optional, resource.IDS_CMD_dir_doc);
            Args.Add(InputArgType.Flag, "recursive", false, Presence.Optional, resource.IDS_CMD_recursive_doc);
            Args.Add(InputArgType.Flag, "single", false, Presence.Optional, resource.IDS_CMD_single_doc);
            Args.Add(InputArgType.RemainingParameters, "FILE {FILE}", null, Presence.Required, resource.IDS_CMD_file_doc);
            Args.Add(InputArgType.Parameter, "env", "PATH", Presence.Optional, resource.IDS_CMD_env_doc);

            if (Args.Process(args))
            {
                Filenames = Args.GetStringList("FILE {FILE}");
                Directories = Args.FindOrCreateStringList("dir");

                string EnvironmentVariableName = Args.GetString("env");
                if (!string.IsNullOrEmpty(EnvironmentVariableName))
                {
                    string env = Environment.GetEnvironmentVariable(EnvironmentVariableName, EnvironmentVariableTarget.User);
                    if( string.IsNullOrEmpty(env) )
                    {
                        env = Environment.GetEnvironmentVariable(EnvironmentVariableName);
                    }
                    EnvironmentVariablesAlreadyChecked[EnvironmentVariableName] = true;
                    if( string.IsNullOrEmpty(env) )
                    {
                        Console.WriteLine(resource.IDS_ERR_invalid_env_var, EnvironmentVariableName);
                    }
                    else
                    {
                        foreach (string token in env.Split(';'))
                        {
                            Directories.Add(token);
                        }
                    }
                }

                if( FilenamesAreIncludes() )
                {
                    AddEnvBasedDirectories("INCLUDE");
                }
                else if (FilenamesAreLibs())
                {
                    AddEnvBasedDirectories("LIB");
                }
                else
                {
                    // default: use standard windows lookup
                    Directories.Add( Directory.GetCurrentDirectory());
                    Directories.Add(PathSanitizer.GetWindowsDirectory());
                    Directories.Add(PathSanitizer.Get32BitSystemDirectory());
                    Directories.Add(Environment.SystemDirectory);
                    Directories.Add(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName));
                    Directories.Add( Environment.GetFolderPath(Environment.SpecialFolder.System));
                    Directories.Add( Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.System)));
                    AddEnvBasedDirectories("PATH");
                }
                
                Directories.MakeUnique(StringComparison.OrdinalIgnoreCase);

                List<string> Extensions = Args.FindOrCreateStringList("extension");
                if (Extensions.Count == 0)
                {
                    foreach (string path in Environment.GetEnvironmentVariable("PATHEXT").Split(';'))
                    {
                        Extensions.Add(path);
                    }
                }

                List<string> FoundItems = new List<string>();
                foreach (string filename in Filenames)
                {
                    bool found = false;
                    foreach (string foundname in Locate(filename))
                    {
                        if (!Contains(FoundItems, foundname))
                        {
                            FileInfo fi = new FileInfo(foundname);

                            Console.WriteLine(resource.IDS_RESULT_PATTERN, 
                                foundname, fi.LastWriteTime, fi.Length);
                            FoundItems.Add(foundname);
                            if (Args.GetFlag("single"))
                                break;

                            found = true;
                        }
                    }
                    if( !found )
                    {
                        Console.WriteLine(resource.IDS_ERR_not_found, filename);
                    }
                }
            }
        }

        private readonly Dictionary<string, bool> EnvironmentVariablesAlreadyChecked = new Dictionary<string,bool>();

        /// <summary>
        /// Add all directories specified in an environment variable
        /// </summary>
        /// <param name="name">Name of the environment variable</param>
        private void AddEnvBasedDirectories(string name)
        {
            if(!EnvironmentVariablesAlreadyChecked.ContainsKey(name))
            {
                EnvironmentVariablesAlreadyChecked[name] = true;
                string content = Environment.GetEnvironmentVariable(name);
                if (!string.IsNullOrEmpty(content))
                {
                    foreach (string path in content.Split(';'))
                    {
                        if (path.Length > 0)
                        {
                            if(!Directories.Contains(path))
                            {
                                Directories.Add(path);
                            }
                        }
                    }
                }
                else Console.WriteLine(resource.IDS_ERR_invalid_env_var, name);
            }
        }

        /// <summary>
        /// Check if the filenames are includes. If so, assume user wants to check the %INCLUDE% variable
        /// </summary>
        /// <returns></returns>
        private bool FilenamesAreIncludes()
        {
            foreach (string filename in Filenames)
            {
                if (GetExtension(filename).Equals(".h", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check if the filenames are libs. If so, assume user wants to check the %LIBS% variable
        /// </summary>
        /// <returns></returns>
        private bool FilenamesAreLibs()
        {
            foreach (string filename in Filenames)
            {
                if (GetExtension(filename).Equals(".lib", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Given a filename, return the extension (or '' if no extension given)
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <returns>Extension</returns>
        private string GetExtension(string filename)
        {
            int k = filename.LastIndexOf('.');
            if (k >= 0)
            {
                return filename.Substring(k).ToLower();
            }
            return "";
        }

        private bool HasWildcard(string filename)
        {
            return (filename.IndexOf('*') >= 0) || (filename.IndexOf('?') >= 0);
        }

        /// <summary>
        /// Check if a particular extension is contained within a list of extensions
        /// </summary>
        /// <param name="extensions">List of extensions</param>
        /// <param name="extension">Extension to find</param>
        /// <returns>True if the extension exists</returns>
        private bool Contains(List<string> extensions, string extension)
        {
            foreach (string e in extensions)
            {
                if (e.Equals(extension, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Locate all instances of the given filename 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private IEnumerable<string> Locate(string filename)
        {
            string originalFilename = filename;

            // if the user is looking for a special 
            List<string> ExtensionsToFind = Args.GetStringList("extension");
            string e = GetExtension(filename);
            if (e.Length > 0)
            {
                ExtensionsToFind = new List<string>(); ;
                ExtensionsToFind.Add(e);
                int k = filename.LastIndexOf('.');
                filename = filename.Substring(0, k);                
            }

            string lookfor = filename;
            if (!lookfor.EndsWith("*"))
                lookfor += "*";

            SearchOption so = SearchOption.TopDirectoryOnly;
            if (Args.GetFlag("recursive"))
                so = SearchOption.AllDirectories;
            
            foreach (string directory in Args.GetStringList("dir"))
            {
                string[] files;
                if( string.IsNullOrEmpty(directory))
                {
                    continue;
                }
                else if (!Directory.Exists(directory))
                {
                    Console.WriteLine(resource.IDS_ERR_dir_does_not_exist, directory);
                    continue;
                }
                try
                {
                    files = Directory.GetFiles(directory, lookfor, so);
                }
                catch (ArgumentException ep)
                {
                    Console.WriteLine(ep);
                    Console.WriteLine(ep.StackTrace);
                    Console.WriteLine(resource.IDS_ERR_while_parsing, directory);
                    Console.WriteLine(resource.IDS_ERR_looking_for, lookfor);
                    break;
                }
                catch (Exception ep)
                {
                    Console.WriteLine(ep);
                    Console.WriteLine(ep.StackTrace);
                    Console.WriteLine(resource.IDS_ERR_while_parsing, directory);
                    continue;
                }
                bool hasWildcard = HasWildcard(originalFilename);
                string lowerFilename = originalFilename.ToLower();

                foreach (string foundname in files)
                {
                    if (!hasWildcard && !foundname.ToLower().Contains(lowerFilename) )
                    {
                        continue;
                    }
                    e = GetExtension(foundname);

                    if (Contains(ExtensionsToFind, e))
                    {
                        yield return foundname;
                    }
                }
            }
        }

        /// <summary>
        /// Main function: defer program logic
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            new Which().Run(args);
        }
    }
}
