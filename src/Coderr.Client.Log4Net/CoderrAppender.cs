﻿using Coderr.Client.ContextCollections;
using Coderr.Client.Contracts;
using Coderr.Client.Log4Net.ContextProviders;
using Coderr.Client.Reporters;
using log4net.Appender;
using log4net.Core;

namespace Coderr.Client.log4net
{
    /// <summary>
    ///     Our appender for logging.
    /// </summary>
    /// <remarks>
    ///     <para>Will upload all log entries that contains exceptions to codeRR.</para>
    /// </remarks>
    public class CoderrAppender : AppenderSkeleton
    {
        /// <inheritdoc />
        protected override bool FilterEvent(LoggingEvent loggingEvent)
        {
            return true;
        }

        /// <summary>
        /// Uploads all log entries that contains an exception to codeRR.
        /// </summary>
        /// <param name="loggingEvent">The logging event.</param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            LogsProvider.Instance.Add(new LogEntryDto(loggingEvent.TimeStampUtc, ConvertLevel(loggingEvent.Level), loggingEvent.RenderedMessage)
            {
                Exception = loggingEvent.ExceptionObject?.ToString(),
                Source = loggingEvent.LoggerName,
            });
            if (loggingEvent.ExceptionObject == null)
                return;

            IErrorReporterContext context = new ErrorReporterContext(this, loggingEvent.ExceptionObject);
            var dataCollection = new LogEntryDetails
            {
                LogLevel = loggingEvent.Level.ToString(),
                Message = loggingEvent.RenderedMessage,
                ThreadName = loggingEvent.ThreadName,
                Timestamp = loggingEvent.TimeStamp,
                LoggerName = loggingEvent.LoggerName,
                UserName = loggingEvent.UserName
            }.ToContextCollection("LogEntry");
            context.ContextCollections.Add(dataCollection);
            
            var coderrCollection = context.ContextCollections.GetCoderrCollection();
            coderrCollection.Properties[CoderrCollectionProperties.HighlightCollection] = "LogEntry";

            Err.Report(context);
        }

        private int ConvertLevel(Level level)
        {
            // Coderr
            // 0 = trace, 1 = debug, 2 = info, 3 = warning, 4 = error, 5 = critical

            // log4net
            //TraceLevel(Level.All); //-2147483648

            //TraceLevel(Level.Verbose);   //  10 000
            //TraceLevel(Level.Finest);    //  10 000

            //TraceLevel(Level.Trace);     //  20 000
            //TraceLevel(Level.Finer);     //  20 000

            //TraceLevel(Level.Debug);     //  30 000
            //TraceLevel(Level.Fine);      //  30 000

            //TraceLevel(Level.Info);      //  40 000
            //TraceLevel(Level.Notice);    //  50 000

            //TraceLevel(Level.Warn);      //  60 000
            //TraceLevel(Level.Error);     //  70 000
            //TraceLevel(Level.Severe);    //  80 000
            //TraceLevel(Level.Critical);  //  90 000
            //TraceLevel(Level.Alert);     // 100 000
            //TraceLevel(Level.Fatal);     // 110 000
            //TraceLevel(Level.Emergency); // 120 000

            if (level == Level.Critical || level == Level.Emergency || level == Level.Fatal)
            {
                return 5;
            }
            
            if (level == Level.Error || level == Level.Severe)
            {
                return 4;
            }
            
            if (level == Level.Warn)
            {
                return 3;
            }
            
            if (level == Level.Info || level == Level.Notice)
            {
                return 2;
            }

            if (level == Level.Debug || level == Level.Fine)
            {
                return 1;
            }

            if (level == Level.Finest || level == Level.Finer || level == Level.Trace)
            {
                return 0;
            }
            return 0;
        }
    }
}