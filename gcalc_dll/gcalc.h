//
//  gcalc.h
//  GKalkEngine
//
//  Created by Gerson Kurz on 01.12.12.
//  Copyright (c) 2012 Gerson Kurz. All rights reserved.
//

#ifndef GKalkEngine_gcalc_h
#define GKalkEngine_gcalc_h

#ifdef _WIN32
#define _CRT_SECURE_NO_WARNINGS

#define gcalc_strdup _strdup
#include "3rdParty\mpir\lib\Win32\Release\gmp.h"
#include "3rdParty\mpfr-3.1.1\src\mpfr.h"

#define strcasecmp strcmp
#define strncasecmp strncmp

#else
#include "/usr/local/include/gmp.h"
#include "/usr/local/include/mpfr.h"
#define gcalc_strdup strdup
#endif

#include "gcalc_typedefs.h"
#include "gcalc_object.h"
#include "gcalc_value.h"
#include "gcalc_variable.h"
#include "gcalc_parser.h"
#include "gcalc_function_macros.h"
#include "gcalc_string_array.h"
#include "gcalc_multiline_output.h"
#include "gcalc_strings.h"

typedef struct s_gcalc_options
{
    struct
    {
        // if this bit is set, then division can promote two integer operands to decimal, for example if you do 1/3.
        // this is the default. If you want "computer" behaviour, do not set this bit.
        int INTEGER_DIVISION_CAN_PROMOTE_TO_DECIMAL;
        
    } flags;
    
    // default is 1.000.000 - which results in a number of over 301.000 decimal digits.
    // coincidently, this is about the best my macpro late-2010 i7 can still comfortably do.
    unsigned long int max_exponent;
    
} gcalc_options;

struct s_gcalc
{
    gcalc_object object_header;
    gcalc_variable* variables;
    gcalc_function_macro* functions;
	const char* last_error_message;
	gcalc_error_code last_error_code;
    gcalc_options options;
};

// create a new gcalc instance. when you're done using this, you should call gcalc_object_release on it.
gcalc* gcalc_create();

gcalc_bool gcalc_get_options(gcalc* calc, gcalc_options* options);
gcalc_bool gcalc_set_options(gcalc* calc, const gcalc_options* options);

// evaluate an expression. if the function returns GCALC_TRUE, then the result is a gcalc_value object,
// on which you must call gcalc_object_release once you're done using it.
gcalc_bool gcalc_evaluate_expression(gcalc* calc, const char* expression, gcalc_value** result);

// set the last error on the gcalc object
gcalc_bool gcalc_set_last_error(gcalc* calc, gcalc_error_code last_error_code, const char* format, ...);

// convert an error message to a string array (for formatted GUI output)
void gcalc_error_to_string_array(gcalc* calc, gcalc_string_array* output);

#endif // GKalkEngine_gcalc_h

