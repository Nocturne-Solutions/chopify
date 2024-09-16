namespace chopify.Helpers
{
    public class AsyncReaderWriterLock
    {
        private readonly SemaphoreSlim _writeSemaphore = new(1, 1);
        private readonly SemaphoreSlim _readSemaphore = new(1, 1);
        private int _readersCount = 0;

        public async Task LockReadAsync()
        {
            await _readSemaphore.WaitAsync();

            try
            {
                if (_readersCount == 0)
                    await _writeSemaphore.WaitAsync();

                _readersCount++;
            }
            finally
            {
                _readSemaphore.Release();
            }
        }

        public async Task UnlockReadAsync()
        {
            await _readSemaphore.WaitAsync();

            try
            {
                _readersCount--;

                if (_readersCount == 0)
                    _writeSemaphore.Release();
            }
            finally
            {
                _readSemaphore.Release();
            }
        }

        public async Task LockWriteAsync()
        {
            await _writeSemaphore.WaitAsync();
        }

        public void UnlockWrite()
        {
            _writeSemaphore.Release();
        }
    }
}
