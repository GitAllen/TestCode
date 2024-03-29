﻿using System;
using System.Diagnostics.Tracing;
using Microsoft.ServiceFabric.Actors;

namespace HelloWorld.ActorService
{
    [EventSource(Name = "MyCompany-HelloWorldApplication-HelloWorldActor")]
    internal sealed class ServiceEventSource : EventSource
    {
        public static ServiceEventSource Current = new ServiceEventSource();

        [NonEvent]
        public void Message(string message, params object[] args)
        {
            string finalMessage = string.Format(message, args);
            this.Message(finalMessage);
        }

        [Event(1, Level = EventLevel.Verbose)]
        public void Message(string message)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(1, message);
            }
        }

        [Event(2, Level = EventLevel.Informational, Message = "Service host {0} registering actor type {1}")]
        public void ActorTypeRegistered(int hostProcessId, string actorType)
        {
            this.WriteEvent(2, hostProcessId, actorType);
        }

        [NonEvent]
        public void ActorHostInitializationFailed(Exception e)
        {
            this.ActorHostInitializationFailed(e.ToString());
        }

        [NonEvent]
        public void ActorActivatedStart(Microsoft.ServiceFabric.Actors.Actor a)
        {
            this.ActorActivatedStart(a.GetType().ToString(), a.Id.ToString(), a.Host.Partition.PartitionInfo.Id);
        }

        [NonEvent]
        public void ActorActivatedStart<T>(Actor<T> a) where T : class
        {
            this.ActorActivatedStart(a.GetType().ToString(), a.Id.ToString(), a.Host.Partition.PartitionInfo.Id);
        }

        [NonEvent]
        public void ActorActivatedStop(Microsoft.ServiceFabric.Actors.Actor a)
        {
            this.ActorActivatedStop(a.GetType().ToString(), a.Id.ToString(), a.Host.Partition.PartitionInfo.Id);
        }

        [NonEvent]
        public void ActorActivatedStop<T>(Actor<T> a) where T : class
        {
            this.ActorActivatedStop(a.GetType().ToString(), a.Id.ToString(), a.Host.Partition.PartitionInfo.Id);
        }

        [NonEvent]
        public void ActorDeactivatedStart(Microsoft.ServiceFabric.Actors.Actor a)
        {
            this.ActorDeactivatedStart(a.GetType().ToString(), a.Id.ToString(), a.Host.Partition.PartitionInfo.Id);
        }

        [NonEvent]
        public void ActorDeactivatedStart<T>(Actor<T> a) where T : class
        {
            this.ActorDeactivatedStart(a.GetType().ToString(), a.Id.ToString(), a.Host.Partition.PartitionInfo.Id);
        }

        [NonEvent]
        public void ActorDeactivatedStop(Microsoft.ServiceFabric.Actors.Actor a)
        {
            this.ActorActivatedStop(a.GetType().ToString(), a.Id.ToString(), a.Host.Partition.PartitionInfo.Id);
        }

        [NonEvent]
        public void ActorDeactivatedStop<T>(Actor<T> a) where T : class
        {
            this.ActorActivatedStop(a.GetType().ToString(), a.Id.ToString(), a.Host.Partition.PartitionInfo.Id);
        }

        [NonEvent]
        public void ActorRequestStart(ActorBase a, string requestName)
        {
            this.ActorRequestStart(a.GetType().ToString(), a.Id.ToString(), requestName);
        }

        [NonEvent]
        public void ActorRequestStop(ActorBase a, string requestName)
        {
            this.ActorRequestStop(a.GetType().ToString(), a.Id.ToString(), requestName);
        }

        [Event(3, Level = EventLevel.Error, Message = "Actor host initialization failed")]
        private void ActorHostInitializationFailed(string exception)
        {
            this.WriteEvent(3, exception);
        }

        [Event(4, Level = EventLevel.Informational, Message = "Actor {1} ({0}) activation start")]
        private void ActorActivatedStart(string actorType, string actorId, Guid partitionId)
        {
            this.WriteEvent(4, actorType, actorId, partitionId);
        }

        [Event(5, Level = EventLevel.Informational, Message = "Actor {1} ({0}) activated")]
        private void ActorActivatedStop(string actorType, string actorId, Guid partitionId)
        {
            this.WriteEvent(5, actorType, actorId, partitionId);
        }

        [Event(6, Level = EventLevel.Informational, Message = "Actor {1} ({0}) deactivation start")]
        private void ActorDeactivatedStart(string actorType, string actorId, Guid partitionId)
        {
            this.WriteEvent(6, actorType, actorId, partitionId);
        }

        [Event(7, Level = EventLevel.Informational, Message = "Actor {1} ({0}) deactivated")]
        private void ActorDeactivatedStop(string actorType, string actorId, Guid partitionId)
        {
            this.WriteEvent(7, actorType, actorId, partitionId);
        }

        [Event(8, Level = EventLevel.Informational, Message = "Actor {1} handling request {2}")]
        private void ActorRequestStart(string actorType, string actorId, string requestName)
        {
            this.WriteEvent(8, actorType, actorId, requestName);
        }

        [Event(9, Level = EventLevel.Informational)]
        private void ActorRequestStop(string actorType, string actorId, string requestName)
        {
            this.WriteEvent(9, actorType, actorId, requestName);
        }
    }
}
