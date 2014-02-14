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

void gcalc_multiline_output_destructor(gcalc_multiline_output* object)
{
    while( object->first )
    {
        gcalc_string_array* next = object->first->next;
        object->first->next = NULL;
        gcalc_object_release( &(object->first->object_header) );
        object->first = next;
    }
    object->last = NULL;
    gcalc_object_release(&(object->calc->object_header));
    object->calc = NULL;
    if( object->formatted_output_string )
    {
        free( object->formatted_output_string);
        object->formatted_output_string = NULL;
    }
}

void gcalc_multiline_output_constructor(gcalc_multiline_output* object, gcalc* calc, gcalc_bool use_for_printing)
{
    gcalc_object_constructor( &(object->object_header), (gcalc_object_destructor) &gcalc_multiline_output_destructor);
    object->first = NULL;
    object->last = NULL;
    object->calc = calc;
    object->use_for_printing = use_for_printing;
    object->formatted_output_string = NULL;
    gcalc_object_retain(&(calc->object_header));
}

gcalc_multiline_output* gcalc_multiline_output_new(gcalc* calc, gcalc_bool use_for_printing)
{
    gcalc_multiline_output* result = (gcalc_multiline_output*) malloc(sizeof(gcalc_multiline_output));
    if( result != NULL )
    {
        gcalc_multiline_output_constructor(result, calc, use_for_printing);
    }
    return result;
}

void gcalc_multiline_output_push_line(gcalc_multiline_output* output, const char* expression)
{
    gcalc_value* result = NULL;
    gcalc_string_array* output_line = NULL;
    
    if( output->use_for_printing )
    {
        // first line is always the expression
        gcalc_string_array* expression_line = gcalc_string_array_new();
        
        // enqueue at end of list
        if( output->last )
        {
            output->last->next = expression_line;
            output->last = expression_line;
        }
        else
        {
            output->first = output->last = expression_line;
        }
        gcalc_string_array_set_string(expression_line, 0, expression);
    }
    
    output_line = gcalc_string_array_new();
    
    // enqueue at end of list
    if( output->last )
    {
        output->last->next = output_line;
        output->last = output_line;
    }
    else
    {
        output->first = output->last = output_line;
    }
 
    if( !expression || !*expression )
    {
        // nothing to see here, move along!
    }
    else
    {
        // evaluate expression
        if( gcalc_evaluate_expression(output->calc, expression, &result) )
        {
            gcalc_value_to_string_array(result, output_line);
            gcalc_object_release(&(result->object_header));
        }
        else
        {
            gcalc_error_to_string_array(output->calc, output_line);
        }
        
        // add empty line
        if( output->use_for_printing )
        {
            // first line is always the expression
            gcalc_string_array* expression_line = gcalc_string_array_new();
        
            // enqueue at end of list
            if( output->last )
            {
                output->last->next = expression_line;
                output->last = expression_line;
            }
            else
            {
                output->first = output->last = expression_line;
            }
            gcalc_string_array_set_string(expression_line, 0, "");
        }
    }
}

#define ALIGN_COLUMNS_WIDTH 12
#define MAX_FORMATTED_OUTPUT_LENGTH (ALIGN_COLUMNS_WIDTH * 5)

typedef struct s_gcalc_template_header
{
    char text[16];
} gcalc_template_header;

const char* gcalc_multiline_output_to_string(gcalc_multiline_output* output)
{
    int i;
    char* buffer;
    gcalc_string_array* p;
    int number_of_lines = 0;
    int chars_needed = 0; // terminating null-byte
    int chars_per_line = 1; // terminating zero
    gcalc_template_header headers[GCALC_MAX_STRING_ARRAY_COLUMNS];
    int max_length_per_cell[GCALC_MAX_STRING_ARRAY_COLUMNS];
    char* writepos; 

    if( output->formatted_output_string )
    {
        free(output->formatted_output_string);
        output->formatted_output_string = NULL;
    }
    
    // calculate max. output length
    
    // reset all lengths
    for(i = 0; i < GCALC_MAX_STRING_ARRAY_COLUMNS; ++i)
        max_length_per_cell[i] = 0;
    
    // calculate string lengths

    for( p = output->first; p; p = p->next )
    {
        p->size_exceeds_output_format = GCALC_FALSE;
        
        if( !p->array[1] && !p->array[2] )
        {
            p->size_exceeds_output_format = GCALC_TRUE;
            if( p->array[0] )
            {
                chars_needed += strlen(p->array[0]);
            }
        }
        else
        {
            for( i = 0; i < GCALC_MAX_STRING_ARRAY_COLUMNS; ++i)
            {
                int n = 0;
                if( p->array[i] != NULL )
                {
                    n = (int) strlen(p->array[i]);
                }
                if( n > MAX_FORMATTED_OUTPUT_LENGTH )
                {
                    chars_needed += n + 2;
                    p->size_exceeds_output_format = GCALC_TRUE;
                }
                if( n > max_length_per_cell[i] )
                {
                    max_length_per_cell[i] = n;
                }
            }
        }
        ++number_of_lines;
    }
    
    // align lengths on boundaries of 4
    
    for( i = 0; i < GCALC_MAX_STRING_ARRAY_COLUMNS; ++i)
    {
        int remainder = max_length_per_cell[i] % ALIGN_COLUMNS_WIDTH;
        if( remainder > 0 )
        {
            max_length_per_cell[i] += ALIGN_COLUMNS_WIDTH - remainder;
        }
        chars_per_line += max_length_per_cell[i] + 1; // length + separator
        sprintf_s(headers[i].text, sizeof(headers[i].text), "%%%ds", max_length_per_cell[i]);
    }
    
    // calculate maximum string size required. this calculation is very rough : but the memory is needed only briefly, and it should still be fine for normal cases
    chars_needed += number_of_lines * (chars_per_line+2);
    
    buffer = (char*) malloc(chars_needed);
    if( buffer == NULL )
        return NULL;

    writepos = buffer;
    for( p = output->first; p; p = p->next )
    {
        if( p->size_exceeds_output_format )
        {
            for( i = 0; i < GCALC_MAX_STRING_ARRAY_COLUMNS; ++i)
            {
                if( p->array[i] )
                {
                    if( i > 0 )
                        *(writepos++) = ' ';
                    strcpy_s(writepos, chars_needed, p->array[i]);
                    writepos += strlen(writepos);
                }
            }
        }
        else
        {
            for( i = 0; i < GCALC_MAX_STRING_ARRAY_COLUMNS; ++i)
            {
                if( i > 0 )
                    *(writepos++) = ' ';
                sprintf_s(writepos, 
                        chars_needed,
                        headers[i].text,
                        p->array[i] ? p->array[i] : "");
                writepos += strlen(writepos);
            }
        }
#ifdef _WIN32
        *(writepos++) = '\r';
#endif
        *(writepos++) = '\n';
        
    }
    *writepos = 0;
    output->formatted_output_string = buffer;
    return output->formatted_output_string;
}

void gcalc_multiline_output_to_callback(gcalc_multiline_output* output, gcalc_multiline_output_callback callback, void* context)
{
    int i; 
    gcalc_string_array* p;
    char* buffer;
    int number_of_lines = 0;
    int chars_needed = 0; // terminating null-byte
    int chars_per_line = 1; // terminating zero
    gcalc_template_header headers[GCALC_MAX_STRING_ARRAY_COLUMNS];
    int max_length_per_cell[GCALC_MAX_STRING_ARRAY_COLUMNS];

    if( output->formatted_output_string )
    {
        free(output->formatted_output_string);
        output->formatted_output_string = NULL;
    }
    
    // calculate max. output length
    
    
    // reset all lengths
    for(i = 0; i < GCALC_MAX_STRING_ARRAY_COLUMNS; ++i)
    {
        max_length_per_cell[i] = 0;
    }

    // calculate string lengths
    
    
    for( p = output->first; p; p = p->next )
    {
        p->size_exceeds_output_format = GCALC_FALSE;
        
        if( !p->array[1] && !p->array[2] )
        {
            p->size_exceeds_output_format = GCALC_TRUE;
            if( p->array[0] )
            {
                chars_needed += strlen(p->array[0]);
            }
        }
        else
        {
            for( i = 0; i < GCALC_MAX_STRING_ARRAY_COLUMNS; ++i)
            {
                int n = 0;
                if( p->array[i] != NULL )
                {
                    n = (int) strlen(p->array[i]);
                }
                if( n > MAX_FORMATTED_OUTPUT_LENGTH )
                {
                    chars_needed += n + 2;
                    p->size_exceeds_output_format = GCALC_TRUE;
                }
                if( n > max_length_per_cell[i] )
                {
                    max_length_per_cell[i] = n;
                }
            }
        }
        ++number_of_lines;
    }
    
    // align lengths on boundaries of 4
    
    for(i = 0; i < GCALC_MAX_STRING_ARRAY_COLUMNS; ++i)
    {
        int remainder = max_length_per_cell[i] % ALIGN_COLUMNS_WIDTH;
        if( remainder > 0 )
        {
            max_length_per_cell[i] += ALIGN_COLUMNS_WIDTH - remainder;
        }
        chars_per_line += max_length_per_cell[i] + 1; // length + separator
        sprintf_s(headers[i].text, sizeof(headers[i].text)/sizeof(char), "%%%ds", max_length_per_cell[i]);
    }
    
    // calculate maximum string size required. this calculation is very rough : but the memory is needed only briefly, and it should still be fine for normal cases
    chars_needed += chars_per_line;
    
    buffer = (char*) malloc(chars_needed);
    if( buffer != NULL )
    {
        for( p = output->first; p; p = p->next )
        {
            char* writepos = buffer;
            if( p->size_exceeds_output_format )
            {
                for( i = 0; i < GCALC_MAX_STRING_ARRAY_COLUMNS; ++i)
                {
                    if( p->array[i] )
                    {
                        if( i > 0 )
                            *(writepos++) = ' ';
                        strcpy_s(writepos, chars_needed, p->array[i]);
                        writepos += strlen(writepos);
                    }
                }
            }
            else
            {
                for( i = 0; i < GCALC_MAX_STRING_ARRAY_COLUMNS; ++i)
                {
                    if( i > 0 )
                        *(writepos++) = ' ';
                    sprintf_s(writepos, chars_needed, headers[i].text,
                            p->array[i] ? p->array[i] : "");
                    writepos += strlen(writepos);
                }
            }
            *writepos = 0;
            callback(buffer, context);
        }
    }
}


