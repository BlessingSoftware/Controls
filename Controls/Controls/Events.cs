using System;
using System.Windows;

namespace BlessingSoftware.Controls {

    public class DateTimeChangedEventArgs :RoutedEventArgs {

        private DateTime? m_oldValue;
        private DateTime? m_newValue;

        public DateTime? NewValue {
            get {
                return this.m_newValue;
            }
        }

        public DateTime? OldValue {
            get {
                return this.m_oldValue;
            }
        }

        public TimeSpan? Delta {
            get {
                if(this.m_newValue.HasValue) {
                    if(this.m_oldValue.HasValue) {
                        return this.m_newValue - this.m_oldValue;
                    }
                }
                return null;
            }
        }

        public DateTimeChangedEventArgs(DateTime? oldValue, DateTime? newValue) {
            this.m_newValue = newValue;
            this.m_oldValue = oldValue;
        }

    }

    public delegate void DateTimeChangedEventHandler(object sender, DateTimeChangedEventArgs e);
}
