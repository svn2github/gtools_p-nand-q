//
//  gcalc_variable.c
//  GKalkEngine
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
#include "gcalc_typedefs.h"
#include "gcalc_value.h"
#include "gcalc_function_macros.h"

void gcalc_function_macro_destructor(gcalc_function_macro* macro)
{
    if( macro->name )
    {
        free((char*)macro->name);
        macro->name = NULL;
    }
    if( macro->expression )
    {
        free((char*)macro->expression);
        macro->expression = NULL;
    }
}

void gcalc_function_macro_constructor(gcalc* calc, gcalc_function_macro* macro,
                                      const char* name, const char* expression, int number_of_arguments)
{
    macro->next = calc->functions;
    calc->functions = macro;
    macro->name = gcalc_strdup(name);
    macro->name_length = (int) strlen(name);
    macro->expression = gcalc_strdup(expression);
    macro->number_of_arguments = number_of_arguments;
}


gcalc_function_macro* gcalc_get_function(gcalc* calc, const char* name)
{
    gcalc_function_macro* f;
    for( f = calc->functions; f; f = f->next )
    {
        if( strcasecmp(f->name, name) == 0 )
        {
            return f;
        }
    }
    return NULL;
}

gcalc_bool gcalc_set_function(gcalc* calc, const char* name, const char* expression, int number_of_arguments)
{
    gcalc_function_macro* result = gcalc_get_function(calc, name);
    if( result )
    {
        if( result->expression )
        {
            free( (char*) result->expression );
        }
        result->expression = gcalc_strdup(expression);
        result->number_of_arguments = number_of_arguments;
        return GCALC_TRUE;
    }
    else
    {
        result = (gcalc_function_macro*) malloc(sizeof(gcalc_function_macro));
        if( result != NULL )
        {
            gcalc_function_macro_constructor(calc, result, name, expression, number_of_arguments);
            return GCALC_TRUE;
        }
        return GCALC_FALSE;
    }
}
