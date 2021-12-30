﻿using System;
using log4net;

namespace Coderr.Client.Log4net.Demo
{
    public class SomeService
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(SomeService));

        public void DoSomeStuff()
        {
            try
            {
                _logger.Debug("Going to invoke the code.");
                throw new AnnoyingException("Something happened when we were not looking.");
            }
            catch (Exception ex)
            {
                _logger.Debug("Failed doing some crazy stuff.", ex);
            }
        }
    }
}