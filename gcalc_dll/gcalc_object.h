//
//  gcalc_object.h
//  GKalkEngine
//
//  Created by Gerson Kurz on 01.12.12.
//  Copyright (c) 2012 Gerson Kurz. All rights reserved.
//

#ifndef GKalkEngine_gcalc_object_h
#define GKalkEngine_gcalc_object_h

typedef struct s_gcalc_object gcalc_object;

typedef void (*gcalc_object_destructor)(gcalc_object* o);

struct s_gcalc_object
{
    int retain_count;
    gcalc_object_destructor destructor;
};

// gcalc object constructor
void gcalc_object_constructor(gcalc_object* p, gcalc_object_destructor destructor);

// retain a reference to a gcalc object
void gcalc_object_retain(gcalc_object* p);

// release a reference to a gcalc object
void gcalc_object_release(gcalc_object* p);

#endif // GKalkEngine_gcalc_object_h


