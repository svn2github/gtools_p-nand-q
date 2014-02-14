//
//  gcalc_strings.c
//  GKalk
//
//  Created by Gerson Kurz on 16.12.12.
//  Copyright (c) 2012 Gerson Kurz. All rights reserved.
//

#include <stdio.h>
#include "gcalc_strings.h"

const char* IDS_GCALC_TRUE = "True";
const char* IDS_GCALC_FALSE = "False";
const char* IDS_GCALC_NOT_IMPLEMENTED_YET = "Not implemented yet";
const char* IDS_GCALC_NULL = "NULL";
const char* IDS_GCALC_ERROR_OPERATION_UNDEFINED_FOR_OPERAND_TYPE = "OPERATION %s UNDEFINED FOR OPERAND TYPE AT POS %d";
const char* IDS_GCALC_ERROR_INVALID_EXPRESSION = "INVALID EXPRESSION AT POS %d: %s";
const char* IDS_GCALC_ERROR_OPERATION_LIMIT_REACHED = "OPERATION LIMIT REACHED AT POS %d: %s";
const char* IDS_GCALC_ERROR_DIVISION_BY_ZERO = "DIVISION BY ZERO AT POS %d";
const char* IDS_GCALC_ERROR_EXPECTED_END_OF_STRING = "EXPECTED END-OF-STRING";
const char* IDS_GCALC_ERROR_MUST_BE_FOLLOWED_BY_VALID_EXPRESSION = "'%s' must be followed by valid expression";
const char* IDS_GCALC_ERROR_MISSING_CLOSING_BRACKET = "missing closing bracket on %s()";
const char* IDS_GCALC_ERROR_SECOND_OPERAND_NOT_RECOGNIZED = "second operand for %s() not recognized";
const char* IDS_GCALC_ERROR_EXPECTED_TWO_ARGUMENTS_GOT_ONE = "%s() expects two arguments, only 1 given";
const char* IDS_GCALC_ERROR_EXPONENT_MUST_BE_LESS_THAN = "exponent must be <= %lu";
const char* IDS_GCALC_FUNCTION_NAMED_POWER = "power";
const char* IDS_GCALC_FUNCTION_NAMED_MODULO = "modulo";
const char* IDS_GCALC_FUNCTION_NAMED_MULTIPLICATION = "multiplication";
const char* IDS_GCALC_FUNCTION_NAMED_DIVISION = "division";
const char* IDS_GCALC_FUNCTION_NAMED_ADDITION = "addition";
const char* IDS_GCALC_FUNCTION_NAMED_SUBTRACTION = "subtraction";
const char* IDS_GCALC_OPERATOR_NAMED_EQUAL = "equal";
const char* IDS_GCALC_OPERATOR_NAMED_NOT_EQUAL = "not-equal";
const char* IDS_GCALC_OPERATOR_NAMED_LESS_THAN = "less-than";
const char* IDS_GCALC_OPERATOR_NAMED_LESS_THAN_OR_EQUAL = "less-than-or-equal";
const char* IDS_GCALC_OPERATOR_NAMED_GREATER_THAN = "greater-than";
const char* IDS_GCALC_OPERATOR_NAMED_GREATER_THAN_OR_EQUAL = "greater-than-or-equal";
