//
//  gcalc_value.c
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
#include <string.h>
#include "gcalc.h"
#include "gcalc_typedefs.h"
#include "gcalc_value.h"
#include "gcalc_parser.h"
#include <limits.h>

static void (*gcalc_gmp_freefunc) (void *, size_t);

void gcalc_value_destructor(gcalc_value* calc)
{
    switch(calc->type)
    {
    case GCALC_TYPE_DECIMAL:
        mpfr_clear(calc->big_decimal);
        break;
    case GCALC_TYPE_INTEGER:
        mpz_clear(calc->big_integer);
        break;
    default:
        ;
    }
    if( calc->last_known_description )
    {
        free((char*)calc->last_known_description);
        calc->last_known_description = NULL;
    }
}

void gcalc_value_set_bool(gcalc_value* value_object, int boolean_value)
{
    gcalc_value_destructor(value_object);
    value_object->boolean = boolean_value ? GCALC_TRUE : GCALC_FALSE;
    value_object->type = GCALC_TYPE_BOOL;
}

const char* gcalc_value_set_last_known_description(gcalc_value* value, const char* output)
{
    if( value->last_known_description )
    {
        free((char*)value->last_known_description);
        value->last_known_description = NULL;
    }
    if( output )
    {
        value->last_known_description = gcalc_strdup(output);
    }
    return value->last_known_description;
}

static void set_gmp_formatted_result_at_index(gcalc_value* value, gcalc_string_array* gsa, const char* fmtstring, int index)
{
    char* output = NULL;
    int chars_used = gmp_asprintf(&output, fmtstring, value->big_integer);
    if( chars_used > 0 )
        gcalc_string_array_set_string(gsa, index, output);
    if( gcalc_gmp_freefunc != NULL )
        gcalc_gmp_freefunc(output, chars_used+1);
}

static void set_mpfr_formatted_result_at_index(gcalc_value* value, gcalc_string_array* gsa, const char* fmtstring, int index)
{
    char* output = NULL;
    mpfr_asprintf(&output, fmtstring, value->big_decimal);
    gcalc_string_array_set_string(gsa, index, output);
    mpfr_free_str(output);
}

static void set_gmp_formatted_binary_result_at_index(gcalc_value* value, gcalc_string_array* gsa, int index)
{
    if( mpz_cmp_si(value->big_integer, 0) < 0 )
    {
        // negative numbers are currently not supported
    }
    else if( mpz_cmp_ui(value->big_integer, ULONG_MAX) > 0 )
    {
        // numbers that are too large are not supported either
    }
    else
    {
        signed long int bit;
        gcalc_bool has_any_bit_been_set;
        char output_buffer[80];
        char* writepos = output_buffer;
        unsigned long int unsigned_value;
        unsigned long int bitmask = 0;
        signed long int max_bits = sizeof(unsigned_value)*8;

        *(writepos++) = '0';
        *(writepos++) = 'b';
        unsigned_value = mpz_get_ui(value->big_integer);
        has_any_bit_been_set = GCALC_FALSE;
        for(  bit = max_bits-1; bit >= 0; --bit )
        {
            bitmask = 1UL << bit;
            if( bitmask & unsigned_value )
            {
                has_any_bit_been_set = GCALC_TRUE;
                *(writepos++) = '1';
            }
            else if( has_any_bit_been_set )
            {
                *(writepos++) = '0';
            }
        }
        if( !has_any_bit_been_set )
            *(writepos++) = '0';
        *writepos = 0;
        gcalc_string_array_set_string(gsa, index, output_buffer);
    }
}

void gcalc_value_to_string_array(gcalc_value* value, gcalc_string_array* gsa)
{   
    if( value == NULL )
    {
        // do nothing - output is all empty. shouldn't reach this point anyway!
    }
    else switch(value->type)
    {
        case GCALC_TYPE_INTEGER:
            set_gmp_formatted_result_at_index(value, gsa, "%Zd", 0);
            set_gmp_formatted_result_at_index(value, gsa, "0x%ZX", 1);
            set_gmp_formatted_binary_result_at_index(value, gsa, 2);
            break;
            
        case GCALC_TYPE_DECIMAL:
            set_mpfr_formatted_result_at_index(value, gsa, "%.10Rg", 0);
            set_mpfr_formatted_result_at_index(value, gsa, "%.5Re", 1);
            break;
            
        case GCALC_TYPE_BOOL:
            gcalc_string_array_set_string(gsa, 0, value->boolean ? IDS_GCALC_TRUE : IDS_GCALC_FALSE);
            break;
            
        default:
            ; // do nothing, this should not happen
    }
}

const char* gcalc_value_to_string(gcalc_value* value)
{
    if( value == NULL )
        return IDS_GCALC_NULL;
    
    switch(value->type)
    {
    case GCALC_TYPE_INTEGER:
        {
            char* output = NULL;
            int chars_used = gmp_asprintf(&output, "%16Zd 0x%016Zx", value->big_integer, value->big_integer);
            const char* result = gcalc_value_set_last_known_description(value, output);
            if( gcalc_gmp_freefunc != NULL )
                gcalc_gmp_freefunc(output, chars_used+1);

            return result;
        }
        break;

    case GCALC_TYPE_DECIMAL:
        {
            const char* result;
            char* output = NULL;
            mpfr_asprintf(&output, "%.32Rf", value->big_decimal);
            result = gcalc_value_set_last_known_description(value, output);
            mpfr_free_str(output);
            return result;
        }
        break;
    
    case GCALC_TYPE_BOOL:
        return value->boolean ? IDS_GCALC_TRUE : IDS_GCALC_FALSE;
    }

    return IDS_GCALC_NOT_IMPLEMENTED_YET;
}

void gcalc_value_constructor(gcalc_value* value)
{
    gcalc_object_constructor( &(value->object_header), (gcalc_object_destructor) &gcalc_value_destructor );
    value->type = GCALC_TYPE_BOOL; // so that no memory is assigned
    value->boolean = GCALC_FALSE;
    if( gcalc_gmp_freefunc == NULL )
    {
        mp_get_memory_functions (NULL, NULL, &gcalc_gmp_freefunc);
    }
    value->last_known_description = NULL;
}

gcalc_value* gcalc_value_clone(gcalc* calc, gcalc_value* source)
{
    gcalc_value* result = (gcalc_value*) malloc(sizeof(gcalc_value));
    if( result != NULL )
    {
        gcalc_value_constructor(result);
        gcalc_value_assign(result, source);
    }
    return result;
}

gcalc_value* gcalc_value_assign(gcalc_value* target, gcalc_value* source)
{
    gcalc_value_destructor(target);   
    target->type = source->type;
    switch(target->type)
    {
        case GCALC_TYPE_DECIMAL:
            mpfr_init_set(target->big_decimal, source->big_decimal, MPFR_DEFAULT_ROUNDING);
            break;
        case GCALC_TYPE_INTEGER:
            mpz_init_set(target->big_integer, source->big_integer);
            break;
        case GCALC_TYPE_BOOL:
            target->boolean = source->boolean;
            break;
        default:
            ;
    }
    return target;
}

gcalc_value* gcalc_value_new_integer(gcalc* calc, const char* value, int base)
{
    gcalc_value* result = (gcalc_value*) malloc(sizeof(gcalc_value));
    if( result != NULL )
    {
        gcalc_value_constructor(result);
        result->type = GCALC_TYPE_INTEGER;
        mpz_init(result->big_integer);
        if( 0 != mpz_set_str (result->big_integer, value, base) )
        {
            // unable to set the result, so we need to fail silently -> In the future, 
            // if gcalc_error is part of the gcalc object, this could be an error condition
            mpz_clear(result->big_integer);
            free(result);
            result = NULL;
        }
    }
    return result;
}

gcalc_value* gcalc_value_new_decimal(gcalc* calc, const char* value, int base)
{
    gcalc_value* result = (gcalc_value*) malloc(sizeof(gcalc_value));
    if( result != NULL )
    {
        gcalc_value_constructor(result);
        result->type = GCALC_TYPE_DECIMAL;
        mpfr_init2(result->big_decimal, MPFR_DEFAULT_PRECISION);
        
        // i have fixed the base to 10 here, because I see litte usage for decimals in other bases - yet;
        // plus doing so here really isolates the issue, should I decide to change my mind at a later point in time.
        if( 0 != mpfr_set_str (result->big_decimal, value, 10, MPFR_DEFAULT_ROUNDING) )
        {
            // unable to set the result, so we need to fail silently
            mpfr_clear(result->big_decimal);
            free(result);
            result = NULL;
        }
    }
    return result;
}

gcalc_value* gcalc_value_new_boolean(gcalc* calc, gcalc_bool value)
{
    gcalc_value* result = (gcalc_value*) malloc(sizeof(gcalc_value));
    if( result != NULL )
    {
        gcalc_value_constructor(result);
        result->type = GCALC_TYPE_BOOL;
        result->boolean = value;
    }
    return result;
}

gcalc_bool gcalc_value_convert_to_decimal(gcalc_parser* context, gcalc_value* value)
{
    switch( value->type )
    {
    case GCALC_TYPE_DECIMAL:
        return GCALC_TRUE;

    case GCALC_TYPE_INTEGER:
        mpfr_init(value->big_decimal);
        mpfr_set_z(value->big_decimal, value->big_integer, MPFR_DEFAULT_ROUNDING);
        mpz_clear(value->big_integer);
        value->type = GCALC_TYPE_DECIMAL;
        return GCALC_TRUE;
    
    case GCALC_TYPE_BOOL:
        mpfr_init(value->big_decimal);
        mpfr_set_si(value->big_decimal, value->boolean ? 1 : 0, MPFR_DEFAULT_ROUNDING);
        value->type = GCALC_TYPE_DECIMAL;
        return GCALC_TRUE;
    }
    return gcalc_parser_set_undefined_operation_for_operand_type(context, "cast-to-decimal");
}

gcalc_bool gcalc_value_set_to_negative(gcalc_parser* context, gcalc_value* value)
{
    switch( value->type )
    {
    case GCALC_TYPE_DECIMAL:
        mpfr_neg(value->big_decimal, value->big_decimal, MPFR_DEFAULT_ROUNDING);
        return GCALC_TRUE;

    case GCALC_TYPE_INTEGER:
        mpz_neg(value->big_integer, value->big_integer);
        return GCALC_TRUE;

    case GCALC_TYPE_BOOL:
        value->boolean = !value->boolean;
        return GCALC_TRUE;
    }
    return gcalc_parser_set_undefined_operation_for_operand_type(context, "negative-value");
}

gcalc_bool gcalc_value_set_to_absolute(gcalc_parser* context, gcalc_value* value)
{
    switch( value->type )
    {
    case GCALC_TYPE_DECIMAL:
        mpfr_abs(value->big_decimal, value->big_decimal, MPFR_DEFAULT_ROUNDING);
        return GCALC_TRUE;

    case GCALC_TYPE_INTEGER:
        mpz_abs(value->big_integer, value->big_integer);
        return GCALC_TRUE;

    case GCALC_TYPE_BOOL:
        return GCALC_TRUE;
    }
    return gcalc_parser_set_undefined_operation_for_operand_type(context, "absolute-value");
}

gcalc_bool gcalc_value_set_to_factorial(gcalc_parser* context, gcalc_value* value)
{
    switch( value->type )
    {
    case GCALC_TYPE_DECIMAL:
        mpz_init(value->big_integer);
        mpz_fac_ui(value->big_integer, mpfr_get_ui(value->big_decimal, MPFR_DEFAULT_ROUNDING));
        mpfr_clear(value->big_decimal);
        return GCALC_TRUE;

    case GCALC_TYPE_INTEGER:
        mpz_fac_ui(value->big_integer, mpz_get_ui(value->big_integer));
        return GCALC_TRUE;

    case GCALC_TYPE_BOOL:
        // 0! = 0, 1! = 1
        return GCALC_TRUE;
    }
    return gcalc_parser_set_undefined_operation_for_operand_type(context, "factorial");
}

gcalc_bool gcalc_value_convert_to_integer(gcalc_parser* context, gcalc_value* value)
{
    switch( value->type )
    {
    case GCALC_TYPE_DECIMAL:
        mpz_init(value->big_integer);
        mpfr_get_z(value->big_integer, value->big_decimal, MPFR_DEFAULT_ROUNDING);
        mpfr_clear(value->big_decimal);
        value->type = GCALC_TYPE_INTEGER;
        return GCALC_TRUE;
    
    case GCALC_TYPE_INTEGER:
        return GCALC_TRUE;

    case GCALC_TYPE_BOOL:
        mpz_init(value->big_integer);
        mpz_set_si(value->big_integer, value->boolean ? 1 : 0);
        value->type = GCALC_TYPE_INTEGER;
        return GCALC_TRUE;
    }
    return gcalc_parser_set_undefined_operation_for_operand_type(context, "cast-to-integer");
}
