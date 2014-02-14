///
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
#include "gcalc_typedefs.h"
#include "gcalc_value.h"
#include "gcalc_variable.h"
#include "gcalc_parser.h"

#define IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(__NAME__) \
\
gcalc_bool gcalc_math_##__NAME__(gcalc_parser* context, gcalc_value* x) \
{\
if( !gcalc_value_convert_to_decimal(context, x) )\
return GCALC_FALSE;\
mpfr_##__NAME__(x->big_decimal, x->big_decimal, MPFR_DEFAULT_ROUNDING);\
return GCALC_TRUE;\
}

IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(sin)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(cos)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(tan)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(log)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(log2)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(log10)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(log1p)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(exp)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(exp2)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(exp10)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(expm1)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(atanh)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(acosh)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(asinh)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(cosh)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(sinh)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(tanh)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(sech)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(csch)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(coth)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(acos)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(asin)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(atan)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(sec)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(csc)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(cot)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(erf)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(erfc)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(sqrt)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(cbrt)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(gamma)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(lngamma)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(digamma)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(zeta)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(j0)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(j1)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(li2)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(y0)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(y1)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(ai)
IMPLEMENT_MPFR_FUNCTION_OF_DECIMAL_X(eint)

static gcalc_unary_function unary_functions[] = {
    { "sin", 3, gcalc_math_sin },
    { "cos", 3, gcalc_math_cos },
    { "tan", 3, gcalc_math_tan },
    { "log", 3, gcalc_math_log },
    { "log2", 4, gcalc_math_log2 },
    { "log10", 5, gcalc_math_log10 },
    { "log1p", 5, gcalc_math_log1p },
    { "exp", 3, gcalc_math_exp },
    { "exp2", 4, gcalc_math_exp2 },
    { "exp10", 5, gcalc_math_exp10 },
    { "expm1", 5, gcalc_math_expm1 },
    { "atanh", 5, gcalc_math_atanh },
    { "acosh", 5, gcalc_math_acosh },
    { "asinh", 5, gcalc_math_asinh },
    { "cosh", 4, gcalc_math_cosh },
    { "sinh", 4, gcalc_math_sinh },
    { "tanh", 4, gcalc_math_tanh },
    { "sech", 4, gcalc_math_sech },
    { "csch", 4, gcalc_math_csch },
    { "coth", 4, gcalc_math_coth },
    { "acos", 4, gcalc_math_acos },
    { "asin", 4, gcalc_math_asin },
    { "atan", 4, gcalc_math_atan },
    { "sec", 3, gcalc_math_sec },
    { "csc", 3, gcalc_math_csc },
    { "cot", 3, gcalc_math_cot },
    { "erf", 3, gcalc_math_erf },
    { "erfc", 4, gcalc_math_erfc },
    { "sqrt", 4, gcalc_math_sqrt },
    { "cbrt", 4, gcalc_math_cbrt },
    { "gamma", 5, gcalc_math_gamma },
    { "lngamma", 7, gcalc_math_lngamma },
    { "digamma", 7, gcalc_math_digamma },
    { "zeta", 4, gcalc_math_zeta },
    { "j0", 2, gcalc_math_j0 },
    { "j1", 2, gcalc_math_j1 },
    { "li2", 3, gcalc_math_li2 },
    { "y0", 2, gcalc_math_y0 },
    { "y1", 2, gcalc_math_y1 },
    { "ai", 2, gcalc_math_ai },
    { "eint", 4, gcalc_math_eint },
    { "int", 3, gcalc_value_convert_to_integer },
    { "float", 5, gcalc_value_convert_to_decimal },
    { NULL }
};

static gcalc_binary_function binary_functions[] = {
    { "gcd", 3, gcalc_math_gcd },
    { NULL }
};

static gcalc_binary_function flevel_pow[] = {
    { "**", 2, gcalc_pow },
    { "^", 1, gcalc_pow },
    { NULL }
};

static gcalc_binary_function flevel_shift[] = {
    { "<<", 2, gcalc_lshift },
    { ">>", 2, gcalc_rshift },
    { NULL }
};

static gcalc_binary_function flevel_add_sub[] = {
    { "+", 1, gcalc_add },
    { "-", 1, gcalc_subtract },
    { NULL }
};

static gcalc_binary_function flevel_mul_mod_div[] = {
    { "*", 1, gcalc_multiply },
    { "/", 1, gcalc_divide },
    { "%", 1, gcalc_modulo },
    { NULL }
};

static gcalc_binary_function flevel_eq_ne_le_ge_leq_geq[] = {
    { "==", 2, gcalc_eq },
    { "<=", 2, gcalc_leq },
    { ">=", 2, gcalc_geq },
    { "<>", 2, gcalc_ne },
    {  "<", 1, gcalc_lt },
    {  ">", 1, gcalc_gt },
    { NULL }
};

static gcalc_functions_on_expression_level levels[] = {
    flevel_eq_ne_le_ge_leq_geq,
    flevel_add_sub,
    flevel_mul_mod_div,
    flevel_pow,
    flevel_shift,
    NULL
};

gcalc_bool gcalc_parser_expression(gcalc_parser* context, gcalc_value** result, int level)
{
    gcalc_value* a = NULL;
    gcalc_value* b = NULL;
    int i;
    int next_level = level+1;
    gcalc_expression_get_arg get_arg = levels[next_level] ? gcalc_parser_expression : gcalc_get_operand;
    
    if( get_arg(context, &a, next_level) )
    {
        gcalc_functions_on_expression_level functions_on_this_level = levels[level];
        gcalc_bool repeat = GCALC_TRUE;
        
        while( repeat )
        {
            repeat = GCALC_FALSE;
            gcalc_parser_skip_whitespaces(context);
            for( i = 0; functions_on_this_level[i].name; ++i )
            {
                if( gcalc_parser_is_operator(context, functions_on_this_level[i].name, functions_on_this_level[i].name_length))
                {
                    gcalc_parser_skip_whitespaces(context);
                    if( get_arg(context, &b, next_level) )
                    {
                        if( !(functions_on_this_level[i].method)(context, a, b) )
                        {
                            gcalc_object_release(&(a->object_header));
                            gcalc_object_release(&(b->object_header));
                            return GCALC_FALSE;
                        }
                        gcalc_object_release(&(b->object_header));
                        repeat = GCALC_TRUE;
                        break;
                    }
                    else
                    {
                        return gcalc_parser_set_invalid_expression(context, IDS_GCALC_ERROR_MUST_BE_FOLLOWED_BY_VALID_EXPRESSION, functions_on_this_level[i].name);
                    }
                }
            }
        }
        *result = a;
        return GCALC_TRUE;
    }
    return GCALC_FALSE;
}

gcalc_bool gcalc_parser_get_expression(gcalc_parser* context, gcalc_value** result)
{
    return gcalc_parser_expression(context, result, 0);
}

gcalc_bool gcalc_parser_is_character(gcalc_parser* context)
{
    char c = *(context->readpos);
    
    if( ((c >= 'A') && (c <= 'Z')) ||
        ((c >= 'a') && (c <= 'z')) ||
        (c == '_') )
    {
        ++(context->readpos);
        return GCALC_TRUE;
    }
    return GCALC_FALSE;
}

gcalc_bool gcalc_parser_is_alphanum(gcalc_parser* context)
{
    char c = *(context->readpos);
    
    if( ((c >= 'A') && (c <= 'Z')) ||
        ((c >= 'a') && (c <= 'z')) ||
        ((c >= '0') && (c <= '9')) ||
        (c == '_') )
    {
        ++(context->readpos);
        return GCALC_TRUE;
    }
    return GCALC_FALSE;
}

gcalc_bool gcalc_parser_is_binary_digit(gcalc_parser* context)
{
    char c = *(context->readpos);
    if( (c >= '0') && (c <= '1'))
    {
        ++(context->readpos);
        return GCALC_TRUE;
    }
    return GCALC_FALSE;
}

gcalc_bool gcalc_parser_is_hexadecimal_digit(gcalc_parser* context)
{
    char c = *(context->readpos);
    if( (c >= '0') && (c <= '9'))
    {
        ++(context->readpos);
        return GCALC_TRUE;
    }
    if( (c >= 'A') && (c <= 'F'))
    {
        ++(context->readpos);
        return GCALC_TRUE;
    }
    if( (c >= 'a') && (c <= 'f'))
    {
        ++(context->readpos);
        return GCALC_TRUE;
    }
    return GCALC_FALSE;
}

gcalc_bool gcalc_parser_is_decimal_digit(gcalc_parser* context)
{
    char c = *(context->readpos);
    if( (c >= '0') && (c <= '9'))
    {
        ++(context->readpos);
        return GCALC_TRUE;
    }
    return GCALC_FALSE;
}

gcalc_bool gcalc_parser_get_assignment(gcalc_parser* context, gcalc_value** result)
{
    const char* start_of_name;
    char* end_of_name;

    gcalc_parser_skip_whitespaces(context);
    start_of_name = context->readpos;

    if( gcalc_parser_is_character(context) )
    {
        while(gcalc_parser_is_alphanum(context))
            ;
        
        end_of_name = (char*) context->readpos;
        gcalc_parser_skip_whitespaces(context);
        if( gcalc_parser_is_operator(context, "=", 1))
        {
            gcalc_parser_skip_whitespaces(context);
            if( gcalc_parser_get_expression(context, result) )
            {
                // OK, now we have an assignment: create it.
                char c = *end_of_name;
                *end_of_name = '\0';
                gcalc_set_variable(context->calc, start_of_name, *result);
                *end_of_name = c;
                return GCALC_TRUE;
            }
        }
        else if( gcalc_parser_is_operator(context, "(x)=", 4) )
        {
            char c = *end_of_name;
            *end_of_name = '\0';
            gcalc_set_function(context->calc, start_of_name, context->readpos, 1);
            *end_of_name = c;
            context->readpos += strlen(context->readpos);
            return GCALC_TRUE;
        }
        else if( gcalc_parser_is_operator(context, "(x,y)=", 6) )
        {
            // TODO: define binary function
        }
        // there are no error conditions here, because of expressions like "pi == pi"
    }
    context->readpos = start_of_name;
    return GCALC_FALSE;
}

gcalc_bool gcalc_parser_evaluate_expression(gcalc_parser* context, gcalc_value** result)
{
    gcalc_parser_skip_whitespaces(context);
    if( gcalc_parser_get_assignment(context, result) ||
        gcalc_parser_get_expression(context, result) )
    {
        gcalc_parser_skip_whitespaces(context);
        if( *context->readpos )
        {
            return gcalc_parser_set_invalid_expression(context, IDS_GCALC_ERROR_EXPECTED_END_OF_STRING);
        }
        return GCALC_TRUE;
    }
    return GCALC_FALSE;
}

gcalc_bool gcalc_lshift(gcalc_parser* context, gcalc_value* a, gcalc_value* b)
{
    mpz_t temp;

    if( !gcalc_value_convert_to_integer(context, a) ||
        !gcalc_value_convert_to_integer(context, b) )
        return GCALC_FALSE;
    
    // (a << b) is the same as a * pow(2, b)
    mpz_init_set_ui(temp, 2);
    mpz_pow_ui(temp, temp, mpz_get_ui(b->big_integer));
    mpz_mul(a->big_integer, a->big_integer, temp);
    mpz_clear(temp);
    return GCALC_TRUE;
}

gcalc_bool gcalc_rshift(gcalc_parser* context, gcalc_value* a, gcalc_value* b)
{
    mpz_t temp;

    if( !gcalc_value_convert_to_integer(context, a) ||
        !gcalc_value_convert_to_integer(context, b) )
        return GCALC_FALSE;
    
    // (a >> b) is the same as a / pow(2, b)
    mpz_init_set_ui(temp, 2);
    mpz_pow_ui(temp, temp, mpz_get_ui(b->big_integer));
    if( mpz_cmp_ui(temp, 0) != 0 )
    {
        mpz_div(a->big_integer, a->big_integer, temp);
    }
    else
    {
        return gcalc_parser_set_division_by_zero(context);
    }
    return GCALC_TRUE;
}

gcalc_bool gcalc_subtract(gcalc_parser* context, gcalc_value* a, gcalc_value* b)
{
    if( !gcalc_parser_align_types(context, a, b))
        return GCALC_FALSE;

    switch(a->type)
    {
    case GCALC_TYPE_INTEGER:
        mpz_sub(a->big_integer, a->big_integer, b->big_integer);
        return GCALC_TRUE;

    case GCALC_TYPE_DECIMAL:
        mpfr_sub(a->big_decimal, a->big_decimal, b->big_decimal, MPFR_DEFAULT_ROUNDING);
        return GCALC_TRUE;

    default:
        return gcalc_parser_set_undefined_operation_for_operand_type(context, IDS_GCALC_FUNCTION_NAMED_SUBTRACTION);
    }
}

// a == b
gcalc_bool gcalc_eq(gcalc_parser* context, gcalc_value* a, gcalc_value* b)
{
    if( !gcalc_parser_align_types(context, a, b))
        return GCALC_FALSE;
    switch(a->type)
    {
        case GCALC_TYPE_INTEGER:
            gcalc_value_set_bool(a, mpz_cmp(a->big_integer, b->big_integer) == 0);
            return GCALC_TRUE;
            
        case GCALC_TYPE_DECIMAL:
            gcalc_value_set_bool(a, mpfr_cmp(a->big_decimal, b->big_decimal) == 0);
            return GCALC_TRUE;
        
        case GCALC_TYPE_BOOL:
            // implicit assumption here: value_object->boolean will be always either GCALC_TRUE or GCALC_FALSE
            gcalc_value_set_bool(a, a->boolean == b->boolean);
            return GCALC_TRUE;
        
        default:
            return gcalc_parser_set_undefined_operation_for_operand_type(context, IDS_GCALC_OPERATOR_NAMED_EQUAL);
    }
}

// a <> b
gcalc_bool gcalc_ne(gcalc_parser* context, gcalc_value* a, gcalc_value* b)
{
    if( !gcalc_parser_align_types(context, a, b))
        return GCALC_FALSE;
    switch(a->type)
    {
        case GCALC_TYPE_INTEGER:
            gcalc_value_set_bool(a, mpz_cmp(a->big_integer, b->big_integer) != 0);
            return GCALC_TRUE;
            
        case GCALC_TYPE_DECIMAL:
            gcalc_value_set_bool(a, mpfr_cmp(a->big_decimal, b->big_decimal) != 0);
            return GCALC_TRUE;
            
        case GCALC_TYPE_BOOL:
            // implicit assumption here: value_object->boolean will be always either GCALC_TRUE or GCALC_FALSE
            gcalc_value_set_bool(a, a->boolean == b->boolean);
            return GCALC_TRUE;
            
        default:
            return gcalc_parser_set_undefined_operation_for_operand_type(context, IDS_GCALC_OPERATOR_NAMED_NOT_EQUAL);
    }
}

// a <= b
gcalc_bool gcalc_leq(gcalc_parser* context, gcalc_value* a, gcalc_value* b)
{
    if( !gcalc_parser_align_types(context, a, b))
        return GCALC_FALSE;
    switch(a->type)
    {
        case GCALC_TYPE_INTEGER:
            gcalc_value_set_bool(a, mpz_cmp(a->big_integer, b->big_integer) <= 0);
            return GCALC_TRUE;
            
        case GCALC_TYPE_DECIMAL:
            gcalc_value_set_bool(a, mpfr_cmp(a->big_decimal, b->big_decimal) <= 0);
            return GCALC_TRUE;
            
        default:
            return gcalc_parser_set_undefined_operation_for_operand_type(context, IDS_GCALC_OPERATOR_NAMED_LESS_THAN_OR_EQUAL);
    }
}

// a >= b
gcalc_bool gcalc_geq(gcalc_parser* context, gcalc_value* a, gcalc_value* b)
{
    if( !gcalc_parser_align_types(context, a, b))
        return GCALC_FALSE;
    switch(a->type)
    {
        case GCALC_TYPE_INTEGER:
            gcalc_value_set_bool(a, mpz_cmp(a->big_integer, b->big_integer) >= 0);
            return GCALC_TRUE;
            
        case GCALC_TYPE_DECIMAL:
            gcalc_value_set_bool(a, mpfr_cmp(a->big_decimal, b->big_decimal) >= 0);
            return GCALC_TRUE;
            
        default:
            return gcalc_parser_set_undefined_operation_for_operand_type(context, IDS_GCALC_OPERATOR_NAMED_GREATER_THAN_OR_EQUAL);
    }
}

// a < b
gcalc_bool gcalc_lt(gcalc_parser* context, gcalc_value* a, gcalc_value* b)
{
    if( !gcalc_parser_align_types(context, a, b))
        return GCALC_FALSE;
    switch(a->type)
    {
        case GCALC_TYPE_INTEGER:
            gcalc_value_set_bool(a, mpz_cmp(a->big_integer, b->big_integer) < 0);
            return GCALC_TRUE;
            
        case GCALC_TYPE_DECIMAL:
            gcalc_value_set_bool(a, mpfr_cmp(a->big_decimal, b->big_decimal) < 0);
            return GCALC_TRUE;
            
        default:
            return gcalc_parser_set_undefined_operation_for_operand_type(context, IDS_GCALC_OPERATOR_NAMED_LESS_THAN);
    }
}

// a > b
gcalc_bool gcalc_gt(gcalc_parser* context, gcalc_value* a, gcalc_value* b)
{
    if( !gcalc_parser_align_types(context, a, b))
        return GCALC_FALSE;
    switch(a->type)
    {
        case GCALC_TYPE_INTEGER:
            gcalc_value_set_bool(a, mpz_cmp(a->big_integer, b->big_integer) > 0);
            return GCALC_TRUE;
            
        case GCALC_TYPE_DECIMAL:
            gcalc_value_set_bool(a, mpfr_cmp(a->big_decimal, b->big_decimal) > 0);
            return GCALC_TRUE;
            
        default:
            return gcalc_parser_set_undefined_operation_for_operand_type(context, IDS_GCALC_OPERATOR_NAMED_GREATER_THAN);
    }
}



gcalc_bool gcalc_add(gcalc_parser* context, gcalc_value* a, gcalc_value* b)
{
    if( !gcalc_parser_align_types(context, a, b))
        return GCALC_FALSE;
    switch(a->type)
    {
    case GCALC_TYPE_INTEGER:
        mpz_add(a->big_integer, a->big_integer, b->big_integer);
        return GCALC_TRUE;

    case GCALC_TYPE_DECIMAL:
        mpfr_add(a->big_decimal, a->big_decimal, b->big_decimal, MPFR_DEFAULT_ROUNDING);
        return GCALC_TRUE;

    default:
        return gcalc_parser_set_undefined_operation_for_operand_type(context, IDS_GCALC_FUNCTION_NAMED_ADDITION);
    }
}

gcalc_bool gcalc_divide(gcalc_parser* context, gcalc_value* a, gcalc_value* b)
{
    if( !gcalc_parser_align_types(context, a, b))
        return GCALC_FALSE;
    switch(a->type)
    {
    case GCALC_TYPE_INTEGER:
        if( mpz_cmp_ui(b->big_integer, 0) != 0 )
        {
            if( context->calc->options.flags.INTEGER_DIVISION_CAN_PROMOTE_TO_DECIMAL )
            {
                // both types are integer - but maybe the division will result in a non-integer
                mpz_t remainder;
                mpz_init_set_ui(remainder, 0);
                
                // calculate remainder. 
                mpz_cdiv_r(remainder, a->big_integer, b->big_integer);
                
                // if there is one, then we need to promote this to decimal
                if( mpz_cmp_ui(remainder, 0) != 0 )
                {
                    if( gcalc_value_convert_to_decimal(context, a) &&
                        gcalc_value_convert_to_decimal(context, b) )
                    {
                        // why do I not check for division by zero here? Because this has already been checked above,
                        // when the input was still all-integers.
                        mpfr_div(a->big_decimal, a->big_decimal, b->big_decimal, MPFR_DEFAULT_ROUNDING);
                        return GCALC_TRUE;
                    }
                    return GCALC_FALSE;
                }
            }
            mpz_cdiv_q(a->big_integer, a->big_integer, b->big_integer);
            return GCALC_TRUE;
        }
        return gcalc_parser_set_division_by_zero(context);

    case GCALC_TYPE_DECIMAL:
        if( mpfr_cmp_ui(b->big_decimal, 0) != 0 )
        {
            mpfr_div(a->big_decimal, a->big_decimal, b->big_decimal, MPFR_DEFAULT_ROUNDING);
            return GCALC_TRUE;
        }
        return gcalc_parser_set_division_by_zero(context);

    default:
        return gcalc_parser_set_undefined_operation_for_operand_type(context, IDS_GCALC_FUNCTION_NAMED_DIVISION);
    }
}

gcalc_bool gcalc_modulo(gcalc_parser* context, gcalc_value* a, gcalc_value* b)
{
    if( !gcalc_parser_align_types(context, a, b))
        return GCALC_FALSE;
    switch(a->type)
    {
    case GCALC_TYPE_INTEGER:
        if( mpz_cmp_ui(b->big_integer, 0) != 0 )
        {
            mpz_cdiv_r(a->big_integer, a->big_integer, b->big_integer);
            return GCALC_TRUE;
        }
        return gcalc_parser_set_division_by_zero(context);

    case GCALC_TYPE_DECIMAL:
        if( mpfr_cmp_ui(b->big_decimal, 0) != 0 )
        {
            // modulo is not "really" defined for decimal numbers.
            // what we do here is this: we convert the numbers to integers, and then to the integer remainder.
            // this therefor changes the type of the variable - but you asked for it, didn't you ;)
            
            if( !gcalc_value_convert_to_integer(context, a) ||
                !gcalc_value_convert_to_integer(context, b) )
                return GCALC_FALSE;
                
            mpz_cdiv_r(a->big_integer, a->big_integer, b->big_integer);
            return GCALC_TRUE;
        }
        return gcalc_parser_set_division_by_zero(context);

    default:
        return gcalc_parser_set_undefined_operation_for_operand_type(context, IDS_GCALC_FUNCTION_NAMED_MODULO);
    }
    return GCALC_TRUE;
}

gcalc_bool gcalc_multiply(gcalc_parser* context, gcalc_value* a, gcalc_value* b)
{
    if( !gcalc_parser_align_types(context, a, b))
        return GCALC_FALSE;
    switch(a->type)
    {
    case GCALC_TYPE_INTEGER:
        mpz_mul(a->big_integer, a->big_integer, b->big_integer);
        return GCALC_TRUE;

    case GCALC_TYPE_DECIMAL:
        mpfr_mul(a->big_decimal, a->big_decimal, b->big_decimal, MPFR_DEFAULT_ROUNDING);
        return GCALC_TRUE;

    default:
        return gcalc_parser_set_undefined_operation_for_operand_type(context, IDS_GCALC_FUNCTION_NAMED_MULTIPLICATION);
    }
}

gcalc_bool gcalc_pow(gcalc_parser* context, gcalc_value* a, gcalc_value* b)
{
    if( !gcalc_parser_align_types(context, a, b))
        return GCALC_FALSE;
    
    if(a->type == GCALC_TYPE_INTEGER)
    {
        long int exponent = mpz_get_si(b->big_integer);
        
        // negative exponent => arguments must be promoted to decimal instead
        if( exponent < 0 )
        {
            if( !gcalc_value_convert_to_decimal(context, a) ||
                !gcalc_value_convert_to_decimal(context, b))
            {
                return GCALC_FALSE;
            }
        }
        else
        {
            if( exponent > (long) context->calc->options.max_exponent )
                return gcalc_parser_set_operation_limit_reached(context, IDS_GCALC_ERROR_EXPONENT_MUST_BE_LESS_THAN, context->calc->options.max_exponent );
            
            mpz_pow_ui(a->big_integer, a->big_integer, exponent);
            return GCALC_TRUE;
        }
    }
    if(a->type == GCALC_TYPE_DECIMAL)
    {
        mpfr_pow(a->big_decimal, a->big_decimal, b->big_decimal, MPFR_DEFAULT_ROUNDING);
        return GCALC_TRUE;
    }
    return gcalc_parser_set_undefined_operation_for_operand_type(context, IDS_GCALC_FUNCTION_NAMED_POWER);
}

gcalc_bool gcalc_math_gcd(gcalc_parser* context, gcalc_value* a, gcalc_value* b)
{
    if( !gcalc_value_convert_to_integer(context, a) ||
        !gcalc_value_convert_to_integer(context, b) )
       return GCALC_FALSE;
    
    // this is an integer function, so I need both arguments as integers 
    mpz_gcd(a->big_integer, a->big_integer, b->big_integer);
    return GCALC_TRUE;
}

gcalc_bool gcalc_parser_set_undefined_operation_for_operand_type(gcalc_parser* context, const char* operation)
{
    return gcalc_set_last_error(context->calc, 
        GCALC_ERROR_OPERATION_UNDEFINED_FOR_OPERAND_TYPE, 
        IDS_GCALC_ERROR_OPERATION_UNDEFINED_FOR_OPERAND_TYPE,
        operation,
        (int)(context->readpos - context->expression));
}

gcalc_bool gcalc_parser_set_invalid_expression(gcalc_parser* context, const char* description, ...)
{
    va_list args;
    char formatted_message[256];

    va_start(args, description);
    vsprintf_s(formatted_message, sizeof(formatted_message) / sizeof(char), description, args);
    
    return gcalc_set_last_error(context->calc, 
        GCALC_ERROR_INVALID_EXPRESSION, 
        IDS_GCALC_ERROR_INVALID_EXPRESSION,
        (int)(context->readpos - context->expression),
        formatted_message);
}

gcalc_bool gcalc_parser_set_operation_limit_reached(gcalc_parser* context, const char* description, ...)
{
    va_list args;
    char formatted_message[256];
    
    va_start(args, description);
    vsprintf_s(formatted_message, sizeof(formatted_message)/sizeof(char), description, args);
    
    return gcalc_set_last_error(context->calc,
                                GCALC_ERROR_OPERATION_LIMIT_REACHED,
                                IDS_GCALC_ERROR_OPERATION_LIMIT_REACHED,
                                (int)(context->readpos - context->expression),
                                formatted_message);
}

gcalc_bool gcalc_parser_set_division_by_zero(gcalc_parser* context)
{
    return gcalc_set_last_error(context->calc, 
        GCALC_ERROR_DIVISION_BY_ZERO, 
        IDS_GCALC_ERROR_DIVISION_BY_ZERO,
        (int)(context->readpos - context->expression));
}

void gcalc_parser_skip_whitespaces(gcalc_parser* context)
{
    while( isspace( *(context->readpos) ) )
    {
        ++(context->readpos);
    }
}

gcalc_bool gcalc_parser_is_operator(gcalc_parser* context, const char* name, int n)
{
    if( strncasecmp(context->readpos, name, n) == 0 )
    {
        context->readpos += n;
        return GCALC_TRUE;
    }
    return GCALC_FALSE;
}

gcalc_bool gcalc_parser_is_hex_indicator(gcalc_parser* context)
{
    char c = *(context->readpos);
    if( (c == 'x') || (c == 'X'))
    {
        ++(context->readpos);
        return GCALC_TRUE;
    }
    return GCALC_FALSE;
}

gcalc_bool gcalc_parser_is_binary_indicator(gcalc_parser* context)
{
    char c = *(context->readpos);
    if( (c == 'b') || (c == 'B'))
    {
        ++(context->readpos);
        return GCALC_TRUE;
    }
    return GCALC_FALSE;
}

gcalc_bool gcalc_get_dec_number(gcalc_parser* context, gcalc_value** result, int ignored)
{
    const char* start_of_string = context->readpos;
    const char* stackpos = context->readpos;
    
    if( *(context->readpos) == '-' )
    {
        ++(context->readpos);
        gcalc_parser_skip_whitespaces(context);
    }
    if( gcalc_parser_is_decimal_digit(context) )
    {
        gcalc_new_gmp_value_func new_value = NULL;
        gcalc_get_digit_method get_digit_method = &gcalc_parser_is_decimal_digit;
        char old;

        if( gcalc_parser_is_hex_indicator(context) )
        {
            get_digit_method = &gcalc_parser_is_hexadecimal_digit;
        }
        else if( gcalc_parser_is_binary_indicator(context) )
        {
            get_digit_method = &gcalc_parser_is_binary_digit;
        }

        // skip over any number of decimals
        while( get_digit_method(context) );

        if( *(context->readpos) == '.' )
        {
            // this looks like a decimal, rather than an integer
            ++(context->readpos);
            
            while( get_digit_method(context) );
            
            new_value = &gcalc_value_new_decimal;
        }
        else
        {
            new_value = &gcalc_value_new_integer;
        }
        old = *(context->readpos);
        *(char*)(context->readpos) = 0;
        *result = new_value(context->calc, start_of_string, 0);
        *(char*)(context->readpos) = old;
        if( *result != NULL )
        {
            if( old == '!' )
            {
                // special case for handling factorial of the form n!
                ++(context->readpos);
                return gcalc_value_set_to_factorial(context, *result);
                
            }
            // NB: originally, I had code there that negated the number if a - sign was present. This has been removed, because
            // the gmp/mpfr string functions handle negative signs already.
            return GCALC_TRUE;
        }
        // TODO: set error here
    }
    context->readpos = stackpos;
    return GCALC_FALSE;
}

gcalc_bool gcalc_get_unary_function(gcalc_parser* context, gcalc_value** result, gcalc_unary_function* function)
{
    const char* readpos = context->readpos;
 
    if( gcalc_parser_is_operator(context, function->name, function->name_length) )
    {
        gcalc_parser_skip_whitespaces(context);
        if( gcalc_parser_is_operator(context, "(", 1 ) )
        {
            // if this is a number, fine
            // if not, we have a parsing error on our hands
            if(gcalc_parser_get_expression(context, result))
            {
                gcalc_parser_skip_whitespaces(context);
                if( gcalc_parser_is_operator(context, ")", 1 ) )
                {
                    return function->method(context, *result);
                }
            }
            return gcalc_parser_set_invalid_expression(context, IDS_GCALC_ERROR_MUST_BE_FOLLOWED_BY_VALID_EXPRESSION, function->name);
        }
    }
    context->readpos = readpos;
    return GCALC_FALSE;
}

gcalc_bool gcalc_get_binary_function(gcalc_parser* context, gcalc_value** result, gcalc_binary_function* function)
{
    const char* readpos = context->readpos;
    
    if( gcalc_parser_is_operator(context, function->name, function->name_length) )
    {
        gcalc_parser_skip_whitespaces(context);
        if( gcalc_parser_is_operator(context, "(", 1 ) )
        {
            // if this is a number, fine
            // if not, we have a parsing error on our hands
            if( gcalc_parser_get_expression(context, result) )
            {
                gcalc_parser_skip_whitespaces(context);
                
                if( gcalc_parser_is_operator(context, ",", 1) )
                {
                    gcalc_value* operand = NULL;

                    gcalc_parser_skip_whitespaces(context);
                    
                    if( gcalc_parser_expression(context, &operand, 0) )
                    {
                        gcalc_parser_skip_whitespaces(context);
                        if( gcalc_parser_is_operator(context, ")", 1 ) )
                        {
                            gcalc_bool b_result = function->method(context, *result, operand);
                            gcalc_object_release(&(operand->object_header));
                            return b_result;
                        }
                        return gcalc_parser_set_invalid_expression(context, IDS_GCALC_ERROR_MISSING_CLOSING_BRACKET, function->name);
                    }
                    return gcalc_parser_set_invalid_expression(context, IDS_GCALC_ERROR_SECOND_OPERAND_NOT_RECOGNIZED, function->name);
                }
                return gcalc_parser_set_invalid_expression(context, IDS_GCALC_ERROR_EXPECTED_TWO_ARGUMENTS_GOT_ONE, function->name);
            }
            return gcalc_parser_set_invalid_expression(context, IDS_GCALC_ERROR_MUST_BE_FOLLOWED_BY_VALID_EXPRESSION, function->name);
        }
    }
    context->readpos = readpos;
    return GCALC_FALSE;
}

gcalc_bool gcalc_parser_is_variable(gcalc_parser* context, gcalc_value** result, gcalc_variable* v)
{
    const char* readpos = context->readpos;
    
    gcalc_parser_skip_whitespaces(context);
    if( gcalc_parser_is_operator(context, v->name, v->name_length) )
    {
        gcalc_parser_skip_whitespaces(context);
        *result = gcalc_value_clone(context->calc, &(v->value) );
        return GCALC_TRUE;
    }
    context->readpos = readpos;
    return GCALC_FALSE;
}

gcalc_bool gcalc_is_user_defined_macro_function(gcalc_parser* context, gcalc_value** result)
{
    const char* start_of_name;
    gcalc_parser_skip_whitespaces(context);
    start_of_name = context->readpos;
    if( gcalc_parser_is_character(context) )
    {
        gcalc_function_macro* function = NULL;
        char* end_of_name;
        char c;

        while(gcalc_parser_is_alphanum(context))
            ;
        
        end_of_name = (char*) context->readpos;
        
        // check if a user-defined function of this name exists
        c = *end_of_name;
        *end_of_name = '\0';
        function = gcalc_get_function(context->calc, start_of_name);
        *end_of_name = c;
        
        if( function == NULL )
        {
            context->readpos = start_of_name;
            return GCALC_FALSE;
        }
        
        gcalc_parser_skip_whitespaces(context);
        if( gcalc_parser_is_operator(context, "(", 1))
        {
            const char* start_of_expression;
            char* end_of_expression;
            int opened_brackets = 0;
            char c;

            gcalc_parser_skip_whitespaces(context);
            start_of_expression = context->readpos;
            
            // TODO: Support functions with more than one argument 
            
            // mini-parser for sub-expression
            end_of_expression = (char*)context->readpos;
            
            while( 1 )
            {
                c = *end_of_expression;
                if( c == '\0' )
                {
                    // bad expression, return error immediately
                    context->readpos = start_of_name;
                    return GCALC_FALSE;
                }
                if( c == '(' )
                {
                    ++opened_brackets;
                }
                else if( c == ')' )
                {
                    --opened_brackets;
                    if( opened_brackets < 0 )
                    {
                        gcalc_parser expression_context;
                        gcalc_bool success;

                        *end_of_expression = '\0';
                        
                        // setup parser to evaluate function argument
                        expression_context.calc = context->calc;
                        expression_context.expression = start_of_expression;
                        expression_context.readpos = start_of_expression;
                        
                        success = gcalc_parser_get_expression(&expression_context, result);
                        *end_of_expression = ')';
                        if( success )
                        {
                            context->readpos = end_of_expression+1;
                            
                            // setup parser with this one variable as input
                            expression_context.expression = function->expression;
                            expression_context.readpos = function->expression;
                            
                            gcalc_push_variable(context->calc, "x", *result);

                            gcalc_object_release(&((*result)->object_header));
                            *result = NULL;
                            success = gcalc_parser_get_expression(&expression_context, result);
                            gcalc_pop_variable(context->calc, "x");
                            return success;
                        }
                        // if the parser fails, that means the expression failed .
                        return GCALC_FALSE;
                    }
                }
                ++end_of_expression;
            }
            // check if this expression can be evaluated
            c = *end_of_name;
            *end_of_name = '\0';
            gcalc_set_variable(context->calc, start_of_name, *result);
            *end_of_name = c;
            return GCALC_TRUE;
        }
    }
    context->readpos = start_of_name;
    return GCALC_FALSE;
}

gcalc_bool gcalc_get_operand(gcalc_parser* context, gcalc_value** result, int ignored)
{
    int i;
    gcalc_variable* v;

    if( gcalc_get_dec_number(context, result, ignored) )
    {
        return GCALC_TRUE;
    }
    if( gcalc_parser_is_operator(context, "(", 1))
    {
        gcalc_parser_skip_whitespaces(context);
        if( gcalc_parser_get_expression(context, result) )
        {
            gcalc_parser_skip_whitespaces(context);
            if( gcalc_parser_is_operator(context,")",1))
            {
                return GCALC_TRUE;
            }
            // TODO: set error message
            return GCALC_FALSE;
        }
        // TODO: else: bracket opened without input
    }
    else if( gcalc_parser_is_operator(context, "|", 1))
    {
        gcalc_parser_skip_whitespaces(context);
        if( gcalc_parser_get_expression(context, result) )
        {
            gcalc_parser_skip_whitespaces(context);
            if( gcalc_parser_is_operator(context,"|",1))
            {
                gcalc_value_set_to_absolute(context, *result);
                return GCALC_TRUE;
            }
            return GCALC_FALSE;
        }
        // TODO: else: bracket opened without input
    }
    
    // this code should be re-written. it should be agnostic to the type of function in the function pointer list, really...
    for( i = 0; unary_functions[i].name; ++i )
    {
        if( gcalc_get_unary_function(context, result, &unary_functions[i]))
        {
            return GCALC_TRUE;
        }
    }
    for( i = 0; binary_functions[i].name; ++i )
    {
        if( gcalc_get_binary_function(context, result, &binary_functions[i]))
        {
            return GCALC_TRUE;
        }
    }
    for( v = context->calc->variables; v; v = v->next )
    {
        if( gcalc_parser_is_variable(context, result, v) )
        {
            return GCALC_TRUE;
        }
    }
    
    if( gcalc_is_user_defined_macro_function(context, result) )
    {
        return GCALC_TRUE;
    }
    // TODO: should raise an error here
    return GCALC_FALSE;
}

gcalc_bool gcalc_parser_align_types(gcalc_parser* context, gcalc_value* a, gcalc_value* b)
{
    if( a->type == b->type )
        return GCALC_TRUE;
    
    // highest type: decimal
    if( a->type == GCALC_TYPE_DECIMAL )
    {
        // some types cannot be aligned to a decimal. skip those. all others are converted to decimals
        if( b->type == GCALC_TYPE_INTEGER )
        {
            mpfr_init(b->big_decimal);
            mpfr_set_z(b->big_decimal, b->big_integer, MPFR_DEFAULT_ROUNDING);
            mpz_clear(b->big_integer);
            b->type = GCALC_TYPE_DECIMAL;
            return GCALC_TRUE;
        }
        if( b->type == GCALC_TYPE_BOOL )
        {
            mpfr_init(b->big_decimal);
            mpfr_set_si(b->big_decimal, b->boolean ? 1 : 0, MPFR_DEFAULT_ROUNDING);
            b->type = GCALC_TYPE_DECIMAL;
            return GCALC_TRUE;
        }
    }
    
    if( a->type == GCALC_TYPE_INTEGER )
    {
        // some types cannot be aligned to a decimal. skip those. all others are converted to decimals
        if( b->type == GCALC_TYPE_DECIMAL )
        {
            mpfr_init(a->big_decimal);
            mpfr_set_z(a->big_decimal, a->big_integer, MPFR_DEFAULT_ROUNDING);
            mpz_clear(a->big_integer);
            a->type = GCALC_TYPE_DECIMAL;
            return GCALC_TRUE;
        }
        if( b->type == GCALC_TYPE_BOOL )
        {
            signed int temp = b->boolean ? 1 : 0;            
            mpz_init(b->big_integer);
            mpz_set_si(b->big_integer, temp);
            b->type = GCALC_TYPE_INTEGER;
            return GCALC_TRUE;
        }
    }
    
    if( a->type == GCALC_TYPE_BOOL)
    {
        // some types cannot be aligned to a decimal. skip those. all others are converted to decimals
        if( b->type == GCALC_TYPE_DECIMAL )
        {
            mpfr_init(a->big_decimal);
            mpfr_set_si(a->big_decimal, a->boolean ? 1 : 0, MPFR_DEFAULT_ROUNDING);
            a->type = GCALC_TYPE_DECIMAL;
            return GCALC_TRUE;
        }
        if( b->type == GCALC_TYPE_INTEGER )
        {
            mpz_init(a->big_integer);
            mpz_set_si(a->big_integer, a->boolean ? 1 : 0);
            a->type = GCALC_TYPE_INTEGER;
            return GCALC_TRUE;
        }
    }
    return GCALC_FALSE;
}

