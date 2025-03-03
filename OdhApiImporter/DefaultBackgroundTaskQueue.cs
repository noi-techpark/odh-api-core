// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OdhApiImporter
{
    public class DefaultBackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

        public DefaultBackgroundTaskQueue(int capacity)
        {
            BoundedChannelOptions options = new(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait,
            };
            _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
        }

        public async ValueTask QueueBackgroundWorkItemAsync(
            Func<CancellationToken, ValueTask> workItem
        )
        {
            if (workItem is null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }
            await _queue.Writer.WriteAsync(workItem);
        }

        public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(
            CancellationToken cancellationToken
        )
        {
            Func<CancellationToken, ValueTask>? workItem = await _queue.Reader.ReadAsync(
                cancellationToken
            );
            return workItem;
        }
    }
}
