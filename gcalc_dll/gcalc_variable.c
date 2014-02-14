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
#include "gcalc_variable.h"

void gcalc_variable_destructor(gcalc_variable* calc)
{
    gcalc_value_destructor(&(calc->value));
    if( calc->name )
    {
        free((char*)calc->name);
        calc->name = NULL;
    }
}

void gcalc_variable_constructor(gcalc* calc, gcalc_variable* variable, const char* name, gcalc_bool is_builtin)
{
    gcalc_value_constructor(&(variable->value));
    variable->next = calc->variables;
    calc->variables = variable;
    variable->name = gcalc_strdup(name);
    variable->name_length = (int) strlen(name);
    variable->is_builtin = is_builtin;
}

gcalc_variable* gcalc_get_variable(gcalc* calc, const char* name)
{
    gcalc_variable* v;

    for( v = calc->variables; v; v = v->next )
    {
        if( strcasecmp(v->name, name) == 0 )
        {
            return v;
        }
    }
    return NULL;
}

void gcalc_pop_variable(gcalc* calc, const char* name)
{
    gcalc_variable* p = NULL;
    gcalc_variable* v;

    for( v = calc->variables; v; v = v->next )
    {
        if( strcasecmp(v->name, name) == 0 )
        {
            if( p == NULL )
                calc->variables = v->next;
            else
                p->next = v->next;
            gcalc_object_release(&(v->value.object_header));
            return;
        }
        p = v;
    }
}

gcalc_bool gcalc_push_variable(gcalc* calc, const char* name, gcalc_value* source)
{
    gcalc_variable* result = (gcalc_variable*) malloc(sizeof(gcalc_variable));
    if( result != NULL )
    {
        gcalc_variable_constructor(calc, result, name, GCALC_FALSE);
        gcalc_value_assign(&(result->value), source);
        return GCALC_TRUE;
    }
    return GCALC_FALSE;
}

gcalc_bool gcalc_set_variable(gcalc* calc, const char* name, gcalc_value* source)
{
    gcalc_variable* result = gcalc_get_variable(calc, name);
    if( result )
    {
        // you cannot overwrite builtin variables! 
        if( result->is_builtin )
            return GCALC_FALSE;

        gcalc_value_assign(&(result->value), source);
        return GCALC_TRUE;
    }
    else
    {
        result = (gcalc_variable*) malloc(sizeof(gcalc_variable));
        if( result != NULL )
        {
            gcalc_variable_constructor(calc, result, name, GCALC_FALSE);
            gcalc_value_assign(&(result->value), source);
            return GCALC_TRUE;
        }
        return GCALC_FALSE;
    }
}

void gcalc_set_builtin_bool_variable(gcalc* calc, const char* name, gcalc_bool boolean_value)
{
    // there is no check here to test if a variable already exists. This is not necessary, because the builtin variables
    // are, well, builtin, and thus do not overlap existing variables.
    gcalc_variable* result = (gcalc_variable*) malloc(sizeof(gcalc_variable));
    if( result != NULL )
    {
        gcalc_variable_constructor(calc, result, name, GCALC_TRUE);
        result->value.type = GCALC_TYPE_BOOL;
        result->value.boolean = boolean_value;
    }
}

void gcalc_set_builtin_decimal_variable(gcalc* calc, const char* name, const char* value)
{
    // there is no check here to test if a variable already exists. This is not necessary, because the builtin variables
    // are, well, builtin, and thus do not overlap existing variables.
    gcalc_variable* result = (gcalc_variable*) malloc(sizeof(gcalc_variable));
    if( result != NULL )
    {
        gcalc_variable_constructor(calc, result, name, GCALC_TRUE);
        result->value.type = GCALC_TYPE_DECIMAL;
        mpfr_init2(result->value.big_decimal, MPFR_DEFAULT_PRECISION);
        
        // i have fixed the base to 10 here, because I see litte usage for decimals in other bases - yet;
        // plus doing so here really isolates the issue, should I decide to change my mind at a later point in time.
        if( 0 != mpfr_set_str (result->value.big_decimal, value, 10, MPFR_DEFAULT_ROUNDING) )
        {
            // remove from queue :/
            calc->variables = result->next;
            gcalc_object_release(&(result->value.object_header));
        }
    }
}

gcalc_bool gcalc_set_decimal_variable(gcalc* calc, const char* name, const char* number)
{
    gcalc_bool result = GCALC_FALSE;
    gcalc_value* value = gcalc_value_new_decimal(calc, number, 10);
    if( value )
    {
        result = gcalc_set_variable(calc, name, value);
        gcalc_object_release(&(value->object_header));
    }
    return result;
}

gcalc_bool gcalc_set_integer_variable(gcalc* calc, const char* name, const char* number)
{
    gcalc_bool result = GCALC_FALSE;
    gcalc_value* value = gcalc_value_new_integer(calc, number, 10);
    if( value )
    {
        result = gcalc_set_variable(calc, name, value);
        gcalc_object_release(&(value->object_header));
    }
    return result;
}


