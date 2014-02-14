//
//  GKalkEngine_gcalc_function_macros.h
//  GKalkEngine
//
//  Created by Gerson Kurz on 01.12.12.
//  Copyright (c) 2012 Gerson Kurz. All rights reserved.
//

#ifndef GKalkEngine_gcalc_function_macros_h
#define GKalkEngine_gcalc_function_macros_h

#include "gcalc_typedefs.h"
#include "gcalc_object.h"

struct s_gcalc_function_macro
{
    // gcalc object header
	gcalc_object header;
    
	// pointer to the next function macro or NULL
    gcalc_function_macro* next;

	// name of the function
    const char* name;
    
    // text of the function
    const char* expression;

	// length of the name of the function.
    int name_length;
    
    // number of arguments: 1 => f(x), 2 => f(x,y), 3 => f(x,y,z)
    int number_of_arguments;
};

// destructor of a gcalc_variable object
void gcalc_function_macro_destructor(gcalc_function_macro* macro);

// common constructor for a gcalc_variable object
void gcalc_function_macro_constructor(gcalc* calc, gcalc_function_macro* macro,
                                      const char* name, const char* expression, int number_of_arguments);

// ---------------------------------------------------------------------------------------------------------------------
// the following methods really belong in the gcalc object, but they are defined here because they deal with the
// creation / access / maintenance of functions 

// define a new function
gcalc_bool gcalc_set_function(gcalc* calc, const char* name, const char* expression, int number_of_arguments);

// get an existing function definition
gcalc_function_macro* gcalc_get_function(gcalc* calc, const char* name);

#endif // GKalkEngine_gcalc_function_macros_h


