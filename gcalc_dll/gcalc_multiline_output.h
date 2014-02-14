//
//  gcalc_value.h
//  GKalkEngine
//
//  Created by Gerson Kurz on 01.12.12.
//  Copyright (c) 2012 Gerson Kurz. All rights reserved.
//

#ifndef GKalkEngine_gcalc_multiline_output_h
#define GKalkEngine_gcalc_multiline_output_h

#include "gcalc_typedefs.h"
#include "gcalc_object.h"
#include "gcalc_string_array.h"

// this structure is a value:
struct s_gcalc_multiline_output
{
    // it has an object header, that describes the instances, handles the retain/release semantics and the destructor
    gcalc_object object_header;
    
    // output string (valid only after gcalc_multiline_output_to_string has been called) 
    char* formatted_output_string;
    
	// it has an object header, that describes the instances, handles the retain/release semantics and the destructor
    gcalc_string_array* first;
    gcalc_string_array* last;
    
    gcalc_bool use_for_printing;
    
    // pointer to owner object
    gcalc* calc;
};

// destructor of a gcalc_value object
void gcalc_multiline_output_destructor(gcalc_multiline_output* object);

// common constructor for a gcalc_value object
void gcalc_multiline_output_constructor(gcalc_multiline_output* object, gcalc* calc, gcalc_bool use_for_printing);

// constructor: new gcalc_value for an integer. input is a string, because it can be arbitrarily long
gcalc_multiline_output* gcalc_multiline_output_new(gcalc* calc, gcalc_bool use_for_printing);

void gcalc_multiline_output_push_line(gcalc_multiline_output* output, const char* expression);

const char* gcalc_multiline_output_to_string(gcalc_multiline_output* output);

typedef void (*gcalc_multiline_output_callback)(const char* line, void* context);

void gcalc_multiline_output_to_callback(gcalc_multiline_output* output, gcalc_multiline_output_callback callback, void* context);

#endif // GKalkEngine_gcalc_multiline_output_h
