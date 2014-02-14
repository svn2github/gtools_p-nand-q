//
//  gcalc_variable.h
//  GKalkEngine
//
//  Created by Gerson Kurz on 01.12.12.
//  Copyright (c) 2012 Gerson Kurz. All rights reserved.
//

#ifndef GKalkEngine_gcalc_variable_h
#define GKalkEngine_gcalc_variable_h

#include "gcalc_typedefs.h"
#include "gcalc_object.h"
#include "gcalc_value.h"

typedef struct s_gcalc_variable
{
	// actual object value
    gcalc_value value;

	// pointer to the next variable or NULL
    struct s_gcalc_variable* next;

	// name of the variable
    const char* name;

	// length of the name of the variable. This is used only during parsing of expressions
    int name_length;

	// flag indicating whether or not the variable is builtin. Builtin variables cannot be over-written
	gcalc_bool is_builtin;
} gcalc_variable;

// destructor of a gcalc_variable object
void gcalc_variable_destructor(gcalc_variable* variable);

// common constructor for a gcalc_variable object
void gcalc_variable_constructor(gcalc* calc, gcalc_variable* variable, const char* name, gcalc_bool is_builtin);

// ---------------------------------------------------------------------------------------------------------------------
// the following methods really belong in the gcalc object, but they are defined here because they deal with the
// creation / access / maintenance of variables 

void gcalc_set_builtin_bool_variable(gcalc* calc, const char* name, gcalc_bool value);
void gcalc_set_builtin_decimal_variable(gcalc* calc, const char* name, const char* value);

// get a variable
gcalc_variable* gcalc_get_variable(gcalc* calc, const char* name);

// push a variable: if in old variable of the same name exists, it is not overwritten, but stored for later usage
gcalc_bool gcalc_push_variable(gcalc* calc, const char* name, gcalc_value* source);

// pop a variable: remove first occurrence of a variable with this name
void gcalc_pop_variable(gcalc* calc, const char* name);

// set a variable. if the variable already exists, it is over-written. If the variable is builtin, it cannot be over-written.
gcalc_bool gcalc_set_variable(gcalc* calc, const char* name, gcalc_value* source);

// define a new decimal variable
gcalc_bool gcalc_set_decimal_variable(gcalc* calc, const char* name, const char* number);

// define a new integer variable
gcalc_bool gcalc_set_integer_variable(gcalc* calc, const char* name, const char* number);

#endif // GKalkEngine_gcalc_variable_h


