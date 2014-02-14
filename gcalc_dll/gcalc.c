//
//  main.c
//  gcalc
//
//  Created by Gerson Kurz on 29.11.12.
//  Copyright (c) 2012 Gerson Kurz. All rights reserved.
//

#include <stdio.h>
#include <memory.h>
#include <stdlib.h>
#include <ctype.h>
#include <stdarg.h>
#include <math.h>
#include <string.h>
#include "gcalc.h"

static void gcalc_destructor(gcalc* calc)
{
    while( calc->variables )
    {
        gcalc_variable* next = calc->variables->next;
        gcalc_object_release(&(calc->variables->value.object_header));
        calc->variables = next;
    }
    while( calc->functions )
    {
        gcalc_function_macro* next = calc->functions->next;
        gcalc_object_release(&(calc->functions->header));
        calc->functions = next;
    }
    if( calc->last_error_message )
    {
        free((char*)calc->last_error_message);
        calc->last_error_message = NULL;
    }
}

gcalc* gcalc_create()
{
    gcalc* result = (gcalc*) malloc(sizeof(gcalc));
    if( result != NULL )
    {
        gcalc_object_constructor(&(result->object_header), (gcalc_object_destructor) &gcalc_destructor);
        result->last_error_message = NULL;
        result->variables = NULL;
        result->last_error_message = NULL;
        result->last_error_code = GCALC_OK;
        result->functions = NULL;
        result->options.flags.INTEGER_DIVISION_CAN_PROMOTE_TO_DECIMAL = GCALC_TRUE;
        result->options.max_exponent = 1000000;
        
        // setup builtin variables
        gcalc_set_builtin_decimal_variable(result, "e", "2.7182818284590452353602874713526624977572470936999595749669676277240766303535475945713821785251664274");
        gcalc_set_builtin_decimal_variable(result, "pi", "3.1415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679");
        gcalc_set_builtin_bool_variable(result, IDS_GCALC_TRUE, GCALC_TRUE);
        gcalc_set_builtin_bool_variable(result, IDS_GCALC_FALSE, GCALC_FALSE);
    }
    return result;
}

gcalc_bool gcalc_get_options(gcalc* calc, gcalc_options* options)
{
    if( options && calc )
    {
        *options = calc->options;
        return GCALC_TRUE;
    }
    return GCALC_FALSE;
}

gcalc_bool gcalc_set_options(gcalc* calc, const gcalc_options* options)
{
    if( options && calc )
    {
        calc->options = *options;
        return GCALC_TRUE;
    }
    return GCALC_FALSE;
}

void gcalc_error_to_string_array(gcalc* calc, gcalc_string_array* output)
{
    gcalc_string_array_set_string(output, 0, calc->last_error_message);
}

gcalc_bool gcalc_set_last_error(gcalc* calc, gcalc_error_code last_error_code, const char* format, ...)
{
    va_list args;
    char buffer[512];
    
    va_start( args, format );
    vsprintf_s(buffer, sizeof(buffer), format, args);
    
    if( calc->last_error_message )
        free((char*)calc->last_error_message);
    calc->last_error_message = gcalc_strdup(buffer);
    calc->last_error_code = last_error_code;
    return GCALC_FALSE;
}

gcalc_bool gcalc_evaluate_expression(gcalc* calc, const char* expression, gcalc_value** result)
{
    gcalc_parser context;
    context.calc = calc;
    context.expression = expression;
    context.readpos = expression;
    return gcalc_parser_evaluate_expression(&context, result);
}

void gcalc_cplusplus_bridge_for_gkalk(gcalc* calc, const char* input_text, char* user_output, int maxsize)
{
    gcalc_multiline_output* output = gcalc_multiline_output_new(calc, GCALC_FALSE);
    
    // create a copy of the input string as a UTF-8 encoded C string
    // why? because we're modifying the string buffer, and I am not 100% sure if the memory we get is writeable
    char* text = _strdup(input_text);

    // start enumerating
    const char* start = NULL;
    char* readpos;
    for( readpos = text; *readpos; ++readpos )
    {
        if( start == NULL )
            start = readpos;
        if( *readpos == '\n')
        {
            *readpos = '\0';
            gcalc_multiline_output_push_line(output, start);
            start = NULL;
        }
    }
    gcalc_multiline_output_push_line(output, start);
    
    strcpy_s(user_output, maxsize, gcalc_multiline_output_to_string(output));
    gcalc_object_release((gcalc_object*)output);
    free(text);
}