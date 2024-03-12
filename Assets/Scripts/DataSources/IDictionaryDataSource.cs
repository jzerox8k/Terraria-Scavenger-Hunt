using System;
using System.Collections.Generic;
using UnityEngine;

public interface IDictionaryDataSource<IKeyType, IValueType>
{
    public Dictionary<IKeyType, IValueType> Data { get; set; }

    /// <summary>
    /// A class uses to transfer data source event information between game objects.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An object that emits these events should derive from this class and implement overrides for the events.
    /// </para>
    /// <para>
    /// An object that subscribes to these events should have an internal member that references the object that will
    /// emit these events.
    /// </para>
    /// </remarks>
    public class EventArguments
    {
        public IDictionaryDataSource<
            IKeyType,
            IValueType
        > DataSource { get; set; }

        public EventArguments(
            IDictionaryDataSource<IKeyType, IValueType> dataSource
        )
        {
            DataSource = dataSource;
        }
    }

    /// <summary>
    /// Used to inform subscribers that the data source they are subscribed to has changed.
    /// </summary>
    public event Action<EventArguments> OnDictionaryDataSourceChanged;

    /// <summary>
    /// Used to inform subscribers that the data source they are subscribed to has been loaded.
    /// </summary>
    public event Action<EventArguments> OnDictionaryDataSourceLoaded;
}
