//
//  gcalc_value.h
//  GKalkEngine
//
//  Created by Gerson Kurz on 01.12.12.
//  Copyright (c) 2012 Gerson Kurz. All rights reserved.
//

#ifndef GKalkEngine_gcalc_string_array_h
#define GKalkEngine_gcalc_string_array_h

#include "gcalc_typedefs.h"
#include "gcalc_object.h"

#define GCALC_MAX_STRING_CACHE                       256
#define GCALC_MAX_STRING_ARRAY_COLUMNS               3

#define GCALC_STRING_ARRAY_COLUMN_INTEGER_DECIMAL    0
#define GCALC_STRING_ARRAY_COLUMN_INTEGER_HEXDECIMAL 1
#define GCALC_STRING_ARRAY_COLUMN_INTEGER_BINARY     2

#define GCALC_STRING_ARRAY_COLUMN_DECIMAL_FLOAT      0
#define GCALC_STRING_ARRAY_COLUMN_DECIMAL_SCIENTIFIC 1

#define GCALC_STRING_ARRAY_COLUMN_BOOLEAN            0

// this structure is a value:
struct s_gcalc_string_array
{
	// it has an object header, that describes the instances, handles the retain/release semantics and the destructor
    gcalc_object object_header;
    
    // next object or NULL
    gcalc_string_array* next;
       
    // max stringmemory used
    int string_cache_used;
    
    // actually, the code will allocate a bigger array
    const char* array[GCALC_MAX_STRING_ARRAY_COLUMNS];
    
    // temporary flag, used by gcalc_multiline_output to handle excessively long output values
    gcalc_bool size_exceeds_output_format;
    
    // cache for the string output
    char string_cache[GCALC_MAX_STRING_CACHE];
};

// destructor of a gcalc_value object
void gcalc_string_array_destructor(gcalc_string_array* object);

// common constructor for a gcalc_value object
void gcalc_string_array_constructor(gcalc_string_array* object);

// constructor: new gcalc_value for an integer. input is a string, because it can be arbitrarily long
gcalc_string_array* gcalc_string_array_new();

// set string array at index
void gcalc_string_array_set_string(gcalc_string_array* object, int index, const char* string);

#endif // GKalkEngine_gcalc_string_array_h

