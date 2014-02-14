//
//  gcalc_value.h
//  GKalkEngine
//
//  Created by Gerson Kurz on 01.12.12.
//  Copyright (c) 2012 Gerson Kurz. All rights reserved.
//

#ifndef GKalkEngine_gcalc_value_h
#define GKalkEngine_gcalc_value_h

#include "gcalc_typedefs.h"
#include "gcalc_object.h"

// possible types of variables
typedef enum
{
    GCALC_TYPE_INTEGER, // integer with arbitrary number of digits, based on GMP
    GCALC_TYPE_DECIMAL, // decimal with extended precision, based on MPFR
    GCALC_TYPE_BOOL,	// boolean, can be 'True' or 'False'
/*
	The following types are scheduled for release in another version, but are not implemented right now:
	
	GCALC_TYPE_FUNCTION, // function - this type is used when functions are defined 
	GCALC_TYPE_COMPLEX,	// complex number
	GCALC_TYPE_CURRENCY,// decimal + assigned currency information
	*/
} gcalc_type;

// this structure is a value:
struct s_gcalc_value
{
	// it has an object header, that describes the instances, handles the retain/release semantics and the destructor
    gcalc_object object_header;

	// type of object, see above
    gcalc_type type;
    
    // this is an arbitrary precision integer as exported by gmp. you cannot do sophisticated math with it, but you can get large integers :)
    mpz_t big_integer;
    
    // this is an arbitrary precision decimal as exported by mpfr.
    mpfr_t big_decimal;
    
    // this is a boring old boolean
    gcalc_bool boolean;

	//  DO NOT USE DIRECTLY: last known string 
	const char* last_known_description;
};

// destructor of a gcalc_value object
void gcalc_value_destructor(gcalc_value* value);

// common constructor for a gcalc_value object
void gcalc_value_constructor(gcalc_value* value);

// the following function is a "naive" output to string, in that the text doesn't know anything about columns.
// for GUI applications, gcalc_value_to_string_array makes much more sense
const char* gcalc_value_to_string(gcalc_value* value);

// this function converts a value to a formatted string output array
void gcalc_value_to_string_array(gcalc_value* value, gcalc_string_array* output);

// constructor: new gcalc_value for an integer. input is a string, because it can be arbitrarily long
gcalc_value* gcalc_value_new_integer(gcalc* calc, const char* value, int base);

// constructor: new gcalc_value for a decimal. input is a string, because it can be arbitrarily long (within limits: there is always the precision rounding)
gcalc_value* gcalc_value_new_decimal(gcalc* calc, const char* value, int base);

// constructor: new gcalc_value for a boolean. 
gcalc_value* gcalc_value_new_boolean(gcalc* calc, gcalc_bool value);

// assign the contents of one value to another, already existing value.
gcalc_value* gcalc_value_assign(gcalc_value* target, gcalc_value* source);

// copy-constructor, basically
gcalc_value* gcalc_value_clone(gcalc* calc, gcalc_value* source);

// given any value, convert it to a decimal (somewhat like an in-place cast operator)
gcalc_bool gcalc_value_convert_to_decimal(gcalc_parser* context, gcalc_value* value);

// given any value, convert it to an integer (somewhat like an in-place cast operator)
gcalc_bool gcalc_value_convert_to_integer(gcalc_parser* context, gcalc_value* value);

// negate a value in-place. if it is a boolean, then return !boolean
gcalc_bool gcalc_value_set_to_negative(gcalc_parser* context, gcalc_value* value);

// get the absolute value, in-place. if the value is a boolean, this function does nothing.
gcalc_bool gcalc_value_set_to_absolute(gcalc_parser* context, gcalc_value* value);

// get the factorial value, in-place. if the value is a boolean, this function does nothing.
gcalc_bool gcalc_value_set_to_factorial(gcalc_parser* context, gcalc_value* value);

// change whatever this variable was before to a boolean value
void gcalc_value_set_bool(gcalc_value* value_object, int boolean_value);

// set last known description
const char* gcalc_value_set_last_known_description(gcalc_value* value, const char* output);

#endif // GKalkEngine_gcalc_value_h



