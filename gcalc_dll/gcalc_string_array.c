//
//  gcalc_string_array.c
//  GKalk
//
//  Created by Gerson Kurz on 16.12.12.
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
#include "gcalc_string_array.h"

static gcalc_bool string_was_allocated_from_cache(gcalc_string_array* object, const char* p)
{
    const char* min_p = &object->string_cache[0];
    const char* max_p = &object->string_cache[object->string_cache_used];
    return (p >= min_p) && (p <= max_p);
}

static void free_string_at_position(gcalc_string_array* object, int i)
{
    if( !string_was_allocated_from_cache(object, object->array[i]))
    {
        free((char*)(object->array[i]));
    }
    object->array[i] = NULL;
}

void gcalc_string_array_destructor(gcalc_string_array* object)
{
    int i;

    for( i = 0; i < GCALC_MAX_STRING_ARRAY_COLUMNS; ++i)
    {
        free_string_at_position(object, i);
    }
    object->string_cache_used = 0;
    while( object->next )
    {
        gcalc_string_array* temp = object->next->next;
        object->next->next = NULL;
        gcalc_object_release( &(object->next->object_header) );
        object->next->next = temp;
    }
}

void gcalc_string_array_set_string(gcalc_string_array* object, int index, const char* string)
{
    if( index < GCALC_MAX_STRING_ARRAY_COLUMNS )
    {
        free_string_at_position(object, index);
        
        if( !string || !*string )
        {
            // string was already set to NULL in free_string_at_position()
        }
        else
        {
            int chars_needed = (int) strlen(string) + 1;
            int available_chars = GCALC_MAX_STRING_CACHE - object->string_cache_used;
            if( available_chars > chars_needed )
            {
                char* target = &(object->string_cache[object->string_cache_used]);
                strcpy_s(target, available_chars, string);
                object->string_cache_used += chars_needed;
                object->array[index] = target;
            }
            else
            {
                object->array[index] = gcalc_strdup(string);
            }
        }
    }
}

void gcalc_string_array_constructor(gcalc_string_array* object)
{
    int i;

    gcalc_object_constructor( &(object->object_header), (gcalc_object_destructor) &gcalc_string_array_destructor);
    object->string_cache_used = 0;
    for( i = 0; i < GCALC_MAX_STRING_ARRAY_COLUMNS; ++i)
    {
        object->array[i] = NULL;
    }
    object->next = NULL;
}

gcalc_string_array* gcalc_string_array_new()
{
    gcalc_string_array* result = (gcalc_string_array*) malloc(sizeof(gcalc_string_array));
    if( result != NULL )
    {
        gcalc_string_array_constructor(result);
    }
    return result;
}
