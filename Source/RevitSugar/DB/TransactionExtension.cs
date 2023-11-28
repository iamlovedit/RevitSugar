using Autodesk.Revit.DB;
using System;

namespace RevitSugar.DB
{
    public static class TransactionExtension
    {
        public static bool RunTransaction(this Document doc, string transactionName, Action action, IFailuresPreprocessor failuresPreprocessor = null)
        {
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            transactionName = string.IsNullOrEmpty(transactionName) ? Guid.NewGuid().ToString() : transactionName;
            using var transaction = new Transaction(doc, transactionName);
            using var handlerOptions = transaction.GetFailureHandlingOptions();
            var transStatus = TransactionStatus.Uninitialized;
            handlerOptions.SetFailuresPreprocessor(failuresPreprocessor ?? new FailuresPreprocessor());
            handlerOptions.SetClearAfterRollback(true);
            handlerOptions.SetDelayedMiniWarnings(false);
            transaction.SetFailureHandlingOptions(handlerOptions);
            try
            {
                transaction.Start();
                action.Invoke();
                transStatus = transaction.Commit();
                return transStatus == TransactionStatus.Committed;
            }
            catch (Exception)
            {
                if (transStatus == TransactionStatus.Started)
                {
                    transaction.RollBack();
                }
                throw;
            }
        }

        public static bool RunTransactionGroup(this Document doc, string transactionName, Action<Document> action)
        {
            if (doc is null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            transactionName = string.IsNullOrEmpty(transactionName) ? Guid.NewGuid().ToString() : transactionName;
            using var transGroup = new TransactionGroup(doc, transactionName);
            var transStatus = TransactionStatus.Uninitialized;
            try
            {
                transGroup.Start();
                action.Invoke(doc);
                transStatus = transGroup.Assimilate();
                return transStatus == TransactionStatus.Committed;
            }
            catch (Exception)
            {
                if (transStatus == TransactionStatus.Started)
                {
                    transGroup.RollBack();
                }
                throw;
            }
        }
    }
}
