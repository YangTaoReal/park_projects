using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ObjectCreater<T> : ICreater<T>
{
    public T Create()
    {
        return (T)Activator.CreateInstance(typeof(T)); ;
    }

    public Task<T> CreateAsync()
    {
        throw new NotImplementedException();
    }
}