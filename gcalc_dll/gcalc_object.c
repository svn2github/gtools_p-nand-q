//
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
#include "gcalc.h"
#include "gcalc_object.h"

void gcalc_object_constructor(gcalc_object* p, gcalc_object_destructor destructor)
{
    p->retain_count = 1;
    p->destructor = destructor;
}

void gcalc_object_retain(gcalc_object* p)
{
    if( p )
    {
        ++(p->retain_count);
    }
}

void gcalc_object_release(gcalc_object* p)
{
    if( p )
    {
        if( p->retain_count > 0 )
        {
            --(p->retain_count);
        }
        if( p->retain_count <= 0 )
        {
            if( p->destructor )
            {
                p->destructor(p);
            }
            free(p);
        }
    }
}


