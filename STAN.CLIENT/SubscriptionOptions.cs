﻿// Copyright 2015-2018 The NATS Authors
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System;

namespace STAN.Client
{
    /// <summary>
    /// The StanSubsciption options class represents various options available
    /// to configure a subscription to a subject on the NATS streaming server.
    /// </summary>
    public class StanSubscriptionOptions
    {
        internal int maxInflight = StanConsts.DefaultMaxInflight;
        internal int ackWait = 30000;
        internal StartPosition startAt = StartPosition.NewOnly;
        internal ulong startSequence = 0;
        internal DateTime startTime;
        internal bool useStartTimeDelta = false;
        internal TimeSpan startTimeDelta;

        internal StanSubscriptionOptions() { }

        internal StanSubscriptionOptions(StanSubscriptionOptions opts)
        {
            if (opts == null)
                return;

            AckWait = opts.AckWait;

            if (opts.DurableName != null)
            {
                DurableName = StanOptions.DeepCopy(opts.DurableName);
            }
            LeaveOpen = opts.LeaveOpen;
            ManualAcks = opts.ManualAcks;
            maxInflight = opts.MaxInflight;
            startAt = opts.startAt;
            startSequence = opts.startSequence;
            useStartTimeDelta = opts.useStartTimeDelta;
            startTime = opts.startTime;
            startTimeDelta = opts.startTimeDelta;
        }

        /// <summary>
        /// DurableName, if set will survive client restarts.
        /// </summary>
        public string DurableName { get; set; }

        /// <summary>
        /// Do Close() on Disposing subscription if true, or Unsubscribe(). If you want to resume subscription with durable name, set true.
        /// </summary>
        /// <remarks>
        /// If Close() or Unsubscribe() is called before Disposing, this flag has no effect
        /// </remarks>
        public bool LeaveOpen { get; set; }

        public void Durable(string name, bool leaveOpen)
        {
            DurableName = name;
            LeaveOpen = leaveOpen;
        }

        /// <summary>
        /// Controls the number of messages the cluster will have inflight without an ACK.
        /// </summary>
        public int MaxInflight
        {
            get { return maxInflight; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "MaxInflight must be greater than 0");

                maxInflight = value;
            }
        }

        /// <summary>
        /// Controls the time the cluster will wait for an ACK for a given message in milliseconds.
        /// </summary>
        /// <remarks>
        /// The value must be at least one second.
        /// </remarks>
        public int AckWait
        {
            get { return ackWait; }
            set
            {
                if (value < 1000)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "AckWait cannot be less than 1000");
 
                ackWait = value;
            }
        }

        /// <summary>
        /// Controls the time the cluster will wait for an ACK for a given message.
        /// </summary>
        public bool ManualAcks { get; set; }

        /// <summary>
        /// Optional start sequence number.
        /// </summary>
        /// <param name="sequence"></param>
        public void StartAt(ulong sequence)
        {
            startAt = StartPosition.SequenceStart;
            startSequence = sequence;    
        }

        /// <summary>
        /// Optional start time. UTC is recommended although a local time will be converted to UTC.
        /// </summary>
        /// <param name="time"></param>
        public void StartAt(DateTime time)
        {
            useStartTimeDelta = false;
            startTime = (time.Kind == DateTimeKind.Utc) ? time :
                time.ToUniversalTime();

            startAt = StartPosition.TimeDeltaStart;
        }

        /// <summary>
        /// Optional start at time delta.
        /// </summary>
        /// <param name="duration"></param>
        public void StartAt(TimeSpan duration)
        {
            useStartTimeDelta = true;
            startTimeDelta = duration;
            startAt = StartPosition.TimeDeltaStart;
        }

        /// <summary>
        /// Start with the last received message.
        /// </summary>
        public void StartWithLastReceived() => startAt = StartPosition.LastReceived;
        
        /// <summary>
        /// Deliver all messages available.
        /// </summary>
        public void DeliverAllAvailable() => startAt = StartPosition.First;

        /// <summary>
        /// Returns a copy of the default subscription options.
        /// </summary>
        public static StanSubscriptionOptions GetDefaultOptions() => new StanSubscriptionOptions();
    }
}
