using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using GSharpTools;
using System.Diagnostics;
using System.Text;
using System.Data.SQLite;

namespace ddupe
{
    class CachedHashes
    {
        private readonly Dictionary<string, string> CacheValues = new Dictionary<string, string>();

        public string LookupHash(string filename)
        {
            filename = filename.ToLower();
            if( CacheValues.ContainsKey(filename) )
                return CacheValues[filename];
            return null;
        }

        /// <summary>
        /// If configured, this function reads from a SQLite database caching previously known hashes.
        /// The motivation for this is that the most expensive operation would be the MD5 hash calculation;
        /// That means that on a second run of DetectDuplicates, we may want to reuse existing known MD5 hashes.
        /// </summary>
        public bool Initialize(string database_filename, Dictionary<long, Dictionary<string, string>> cache)
        {
            if (string.IsNullOrEmpty(database_filename))
                return false;

            Filename = database_filename;

            try
            {
                // connect to database
                Console.WriteLine(resource.IDS_reading_cache, Filename);
                Database = new SQLiteConnection("Data Source=" + Filename);

                // make sure lookup table exists
                ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS hashes (hash TEXT, filename TEXT);");

                DateTime cacheStartTime = DateTime.Now;

                // read known hashes from lookup table
                int CachedHashesRead = 0;
                DbCommand command = new SQLiteCommand("SELECT hash, filename FROM hashes");
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string hash = reader[0] as string;
                        string filename = reader[1] as string;
                        CacheValues[filename.ToLower()] = hash;
                        ++CachedHashesRead;
                    }
                }
                if (CachedHashesRead == 0)
                {
                    Console.WriteLine(resource.IDS_cache_is_empty);
                }
                else
                {
                    TimeSpan elapsed = DateTime.Now - cacheStartTime;
                    Console.WriteLine(resource.IDS_cache_read, CachedHashesRead, elapsed);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(resource.IDS_ERR_db_read, database_filename);
                return false;
            }
        }

        public void Flush()
        {
            if (Database != null)
            {
                if (Transaction != null)
                {
                    Debug.Assert(Command != null);
                    Debug.Assert(SizeUsed > 0);

                    Transaction.Commit();
                    Command.Dispose();
                    Command = null;
                    Transaction.Dispose();
                    Transaction = null;
                    SizeUsed = 0;
                }
            }
        }

        public void Write(string hash, string filename)
        {
            if (Database != null)
            {
                // create transaction object if it doesn't exist yet
                if( Transaction == null )
                {
                    Debug.Assert(SizeUsed == 0);
                    Debug.Assert(Command == null);

                    Transaction = Database.BeginTransaction();
                    Command = new SQLiteCommand("INSERT INTO hashes (hash, filename) VALUES (?,?)");

                    Field_HashText = Command.CreateParameter();
                    Field_FileName = Command.CreateParameter();

                    Command.Parameters.Add(Field_HashText);
                    Command.Parameters.Add(Field_FileName);
                }

                Field_HashText.Value = hash;
                Field_FileName.Value = filename;
                Command.ExecuteNonQuery();

                ++SizeUsed;
                if (SizeUsed >= FLUSH_SIZE)
                {
                    Flush();
                }
            }
        }

        private int ExecuteNonQuery(string statement)
        {
            if (statement == null || statement.Equals(""))
                return 0;

            SQLiteCommand command = new SQLiteCommand(statement);
            command.CommandTimeout = 0;
            try
            {
                return command.ExecuteNonQuery();
            }
            catch (DbException e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(resource.IDS_ERR_execute_statement, statement);
                throw e;
            }
        }

        /// <summary>
        /// Filename for database
        /// </summary>
        private string Filename;

        private SQLiteParameter Field_HashText;
        private SQLiteParameter Field_FileName;

        /// <summary>
        /// Connection to SQLite database
        /// </summary>
        private SQLiteConnection Database;

        /// <summary>
        /// Transaction used to speed up the processing
        /// </summary>
        private SQLiteTransaction Transaction;

        /// <summary>
        /// Command used during an active transaction
        /// </summary>
        private SQLiteCommand Command;

        /// <summary>
        /// flush transaction every 10000 elements
        /// </summary> 
        private const int FLUSH_SIZE = 1000;

        /// <summary>
        /// current size of elements in cache 
        /// </summary>
        private int SizeUsed = 0;
    }
}
