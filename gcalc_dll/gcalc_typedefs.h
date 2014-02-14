//
//  gcalc_typedefs.h
//  GKalkEngine
//
//  Created by Gerson Kurz on 01.12.12.
//  Copyright (c) 2012 Gerson Kurz. All rights reserved.
//

#ifndef GKalkEngine_gcalc_typedefs_h
#define GKalkEngine_gcalc_typedefs_h

typedef struct s_gcalc gcalc;
typedef struct s_gcalc_value gcalc_value;
typedef struct s_gcalc_parser gcalc_parser;
typedef struct s_gcalc_function_macro gcalc_function_macro;
typedef struct s_gcalc_string_array gcalc_string_array;
typedef struct s_gcalc_multiline_output gcalc_multiline_output;

typedef char gcalc_bool;

#define GCALC_TRUE  ((gcalc_bool)1)
#define GCALC_FALSE ((gcalc_bool)0)

typedef enum 
{
	GCALC_OK = 0,
    GCALC_ERROR_DIVISION_BY_ZERO,
    GCALC_ERROR_INVALID_EXPRESSION,
	GCALC_ERROR_OPERATION_UNDEFINED_FOR_OPERAND_TYPE,
    GCALC_ERROR_UNABLE_TO_OVERWRITE_BUILTIN_VARIABLE,
    GCALC_ERROR_OPERATION_LIMIT_REACHED,
} gcalc_error_code;

#endif // GKalkEngine_gcalc_typedefs_h
