namespace roundhouse.databases.advantage
{
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using System;

    public class TransientErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        public bool IsTransient(Exception ex) => false;
    }
}