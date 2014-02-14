//
//  gcalc_parser.h
//  GKalkEngine
//
//  Created by Gerson Kurz on 01.12.12.
//  Copyright (c) 2012 Gerson Kurz. All rights reserved.
//

#ifndef GKalkEngine_gcalc_parser_h
#define GKalkEngine_gcalc_parser_h

#include "gcalc.h"
#include "gcalc_typedefs.h"
#include "gcalc_value.h"
#include "gcalc_variable.h"
#include "gcalc_parser.h"

struct s_gcalc_parser
{
    gcalc* calc;
    const char* expression;
    const char* readpos;
};

typedef gcalc_value* (*gcalc_new_gmp_value_func)(gcalc* calc, const char* value, int base);
typedef gcalc_bool (*gcalc_unary_method)(gcalc_parser* context, gcalc_value* a);
typedef gcalc_bool (*gcalc_binary_method)(gcalc_parser* context, gcalc_value* a, gcalc_value* b);
typedef gcalc_bool (*gcalc_expression_get_arg)(gcalc_parser* context, gcalc_value** result, int level);
typedef gcalc_bool (*gcalc_get_digit_method)(gcalc_parser* context);

typedef struct 
{
    const char* name;
    int name_length;
    gcalc_unary_method method;
} gcalc_unary_function;

typedef struct 
{
    const char* name;
    int name_length;
    gcalc_binary_method method;
} gcalc_binary_function, *gcalc_functions_on_expression_level;

#define DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(__NAME__) gcalc_bool gcalc_math_##__NAME__(gcalc_parser* context, gcalc_value* x);

// mostly trigonometric - refer to MPFR documentation for details
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(sin)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(cos)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(tan)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(log)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(log2)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(log10)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(log1p)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(exp)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(exp2)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(exp10)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(expm1)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(atanh)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(acosh)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(asinh)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(cosh)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(sinh)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(tanh)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(sech)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(csch)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(coth)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(acos)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(asin)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(atan)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(sec)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(csc)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(cot)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(erf)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(erfc)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(sqrt)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(cbrt)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(gamma)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(lngamma)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(digamma)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(zeta)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(j0)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(j1)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(li2)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(y0)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(y1)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(ai)
DECLARE_MPFR_FUNCTION_OF_DECIMAL_X(eint)

// surely there are better choices for these, but I don't know them yet:
#define MPFR_DEFAULT_PRECISION 	512
#define MPFR_DEFAULT_ROUNDING 	MPFR_RNDN

// function prototypes for the internal functions. None of these are visible to external callers..

// x or X, indicating a hex number
gcalc_bool gcalc_parser_is_hex_indicator(gcalc_parser* context);

// b or B, indicating a binary number
gcalc_bool gcalc_parser_is_binary_indicator(gcalc_parser* context);

// this function evaluates an expression, and returns a result
gcalc_bool gcalc_parser_evaluate_expression(gcalc_parser* context, gcalc_value** result);

// get an assignment, either for variables or for functions. 
// The result of a variable assignment is the value of the variable. 
// The result of a function assignment is a function-type value. 
gcalc_bool gcalc_parser_get_assignment(gcalc_parser* context, gcalc_value** result);

// if the current character is alphanumeric, go to the next one and return true, otherwise return false
gcalc_bool gcalc_parser_is_alphanum(gcalc_parser* context);

// if the current character is alphabetic (non-numeric), go to the next one and return true, otherwise return false
gcalc_bool gcalc_parser_is_character(gcalc_parser* context);

// if the current character is a decimal number (0-9), go to the next one and return true, otherwise return false
gcalc_bool gcalc_parser_is_decimal_digit(gcalc_parser* context);

// if the current character is a hexadecimal number (0-9, A-F or a-f), go to the next one and return true, otherwise return false
gcalc_bool gcalc_parser_is_hexadecimal_digit(gcalc_parser* context);

// if the current character is a binary number (0-1), go to the next one and return true, otherwise return false
gcalc_bool gcalc_parser_is_binary_digit(gcalc_parser* context);

// get the next expression
gcalc_bool gcalc_parser_get_expression(gcalc_parser* context, gcalc_value** result);

// get an expression on a certain level
gcalc_bool gcalc_parser_expression(gcalc_parser* context, gcalc_value** result, int level);

// skip whitespaces
void gcalc_parser_skip_whitespaces(gcalc_parser* context);

// check if there is an operator at the current read position
gcalc_bool gcalc_parser_is_operator(gcalc_parser* context, const char* name, int n);

// get decimal number at current read position
gcalc_bool gcalc_get_dec_number(gcalc_parser* context, gcalc_value** result, int ignored);

// get unary function at current read position
gcalc_bool gcalc_get_unary_function(gcalc_parser* context, gcalc_value** result, gcalc_unary_function* function);

// get binary function at current read position
gcalc_bool gcalc_get_binary_function(gcalc_parser* context, gcalc_value** result, gcalc_binary_function* function);

// get operand at current read position
gcalc_bool gcalc_get_operand(gcalc_parser* context, gcalc_value** result, int ignored);

// check if there is a variable reference at the current read position
gcalc_bool gcalc_parser_is_variable(gcalc_parser* context, gcalc_value** result, gcalc_variable* v);

// given two input variables, align their types so that the "best" type wins. 
// for example, 1+1.0 will result in a decimal, not an integer
gcalc_bool gcalc_parser_align_types(gcalc_parser* context, gcalc_value* a, gcalc_value* b);

// return result of evaluating user-defined macro function
gcalc_bool gcalc_is_user_defined_macro_function(gcalc_parser* context, gcalc_value** result);

// -------------------------------------------------------------------------------------------------------------------------
// mathematical operations

// a <<= b
gcalc_bool gcalc_rshift(gcalc_parser* context, gcalc_value* a, gcalc_value* b);

// a >>= b
gcalc_bool gcalc_lshift(gcalc_parser* context, gcalc_value* a, gcalc_value* b);

// a -= b
gcalc_bool gcalc_subtract(gcalc_parser* context, gcalc_value* a, gcalc_value* b);

// a += b
gcalc_bool gcalc_add(gcalc_parser* context, gcalc_value* a, gcalc_value* b);

// a /= b
gcalc_bool gcalc_divide(gcalc_parser* context, gcalc_value* a, gcalc_value* b);

// a %= b
gcalc_bool gcalc_modulo(gcalc_parser* context, gcalc_value* a, gcalc_value* b);

// a *= b
gcalc_bool gcalc_multiply(gcalc_parser* context, gcalc_value* a, gcalc_value* b);

// a = pow(a, b);
gcalc_bool gcalc_pow(gcalc_parser* context, gcalc_value* a, gcalc_value* b);

// a = gcd(a, b)
gcalc_bool gcalc_math_gcd(gcalc_parser* context, gcalc_value* a, gcalc_value* b);

// a == b
gcalc_bool gcalc_eq(gcalc_parser* context, gcalc_value* a, gcalc_value* b);

// a <> b
gcalc_bool gcalc_ne(gcalc_parser* context, gcalc_value* a, gcalc_value* b);

// a <= b
gcalc_bool gcalc_leq(gcalc_parser* context, gcalc_value* a, gcalc_value* b);

// a >= b
gcalc_bool gcalc_geq(gcalc_parser* context, gcalc_value* a, gcalc_value* b);

// a < b
gcalc_bool gcalc_lt(gcalc_parser* context, gcalc_value* a, gcalc_value* b);

// a > b
gcalc_bool gcalc_gt(gcalc_parser* context, gcalc_value* a, gcalc_value* b);

// -------------------------------------------------------------------------------------------------------------------------
// error handling

// create a GCALC_ERROR_INVALID_EXPRESSION at the current parsing position
gcalc_bool gcalc_parser_set_invalid_expression(gcalc_parser* context, const char* description, ...);

// create a GCALC_ERROR_DIVISION_BY_ZERO at the current parsing position
gcalc_bool gcalc_parser_set_division_by_zero(gcalc_parser* context);

// create a GCALC_ERROR_OPERATION_UNDEFINED_FOR_OPERAND_TYPE at the current parsing position
gcalc_bool gcalc_parser_set_undefined_operation_for_operand_type(gcalc_parser* context, const char* operation);

// create a GCALC_ERROR_OPERATION_LIMIT_REACHED at the current parsing position
gcalc_bool gcalc_parser_set_operation_limit_reached(gcalc_parser* context, const char* description, ...);

#endif // GKalkEngine_gcalc_parser_h

