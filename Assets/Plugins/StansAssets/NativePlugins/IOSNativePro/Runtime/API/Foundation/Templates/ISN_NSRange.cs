using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.iOS.Foundation
{
    /// <summary>
    /// A class used to describe a portion of a series, 
    /// such as characters in a string or objects in an array.
    /// </summary>
    [Serializable]
    public class ISN_NSRange 
    {
        [SerializeField] long m_location;
        [SerializeField] long m_length;


        /// <summary>
        /// Initializes a new instance of the <see cref="ISN_NSRange"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="length">Length.</param>
        public ISN_NSRange(long location, long length) {
            m_location = location;
            m_length = length;
        }

        /// <summary>
        /// The start index (0 is the first, as in C arrays). 
        /// For type compatibility with the rest of the system, 
        /// <see cref=" long.MaxValue"/> is the maximum value you should use for location.
        /// </summary>
        /// <value>The location.</value>
        public long Location {
            get {
                return m_location;
            }

            set {
                m_location = value;
            }
        }

        /// <summary>
        /// The number of items in the range (can be 0). 
        /// For type compatibility with the rest of the system, 
        /// <see cref=" long.MaxValue"/> is the maximum value you should use for length.
        /// </summary>
        public long Length {
            get {
                return m_length;
            }

            set {
                m_length = value;
            }
        }
    }
}