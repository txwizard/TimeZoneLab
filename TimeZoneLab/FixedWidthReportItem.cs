/*
    ============================================================================

    Namespace:          TimeZoneLab

    Class Name:         FixedWidthReportItem

    File Name:          FixedWidthReportItem.cs

    Synopsis:           Instances of this class represent individual columns to
                        be included in a fixed width report.

    Remarks:            A FixedWidthReportItemsCollection of these objects is
                        used to organize columns of data into a report.

	License:            Copyright (C) 2014-2016, David A. Gray.
						All rights reserved.

                        Redistribution and use in source and binary forms, with
                        or without modification, are permitted provided that the
                        following conditions are met:

                        *   Redistributions of source code must retain the above
                            copyright notice, this list of conditions and the
                            following disclaimer.

                        *   Redistributions in binary form must reproduce the
                            above copyright notice, this list of conditions and
                            the following disclaimer in the documentation and/or
                            other materials provided with the distribution.

                        *   Neither the name of David A. Gray, nor the names of
                            his contributors may be used to endorse or promote
                            products derived from this software without specific
                            prior written permission.

                        THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
                        CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
                        WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
                        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
                        PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
                        David A. Gray BE LIABLE FOR ANY DIRECT, INDIRECT,
                        INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
                        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
                        SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
                        PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
                        ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
                        LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
                        ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
                        IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

    Created:            Sunday, 31 August 2014 and Monday, 01 September 2014

    ----------------------------------------------------------------------------
    Revision History
    ----------------------------------------------------------------------------

    Date       Version Author Description
    ---------- ------- ------ --------------------------------------------------
    2014/09/01 1.0     DAG    Initial implementation.

	2021/10/09 3.0     DAG    1) Implement time zome abbreviation generation.

							  2) Upgrade to current libraries via NuGet.
    ============================================================================
*/


using System;
using System.Collections.Generic;
using System.Text;

using WizardWrx;


namespace TimeZoneLab
{
    class FixedWidthReportItem : IComparable
    {
        #region Public Constants
        public const int INVALID_WIDTH = ListInfo.EMPTY_STRING_LENGTH;
        public const int INVALID_POSITION = ArrayInfo.ARRAY_INVALID_INDEX;

        public const char LBL_GUIDE_USE_DEFAULT = SpecialCharacters.NULL_CHAR;
		public const char LBL_GUIDE_DEFAULT = WizardWrx.SpecialCharacters.HYPHEN;

        public const string ERRMSG_INCOMPARABLE = @"Both objects must be of the same type.{2}Type of THIS object  = {0}{2}Type of OTHER object = {1}";
        public const string COL_POS_REQUIREMENTS = @"Value must be a valid array subsscript, because it is used as such internally.";
        public const string LABEL_REQUIREMENTS = @"The {0} cannot be a null reference (Nothing in Visual Basic) or the empty string.";
        #endregion  // Public Constants


        #region Static Members
        static Type s_typOfMe = typeof ( FixedWidthReportItem );
        #endregion  // Static Members


        #region Constructors
        /// <summary>
        /// Create an empty instance. Using this constructor is discouraged.
        /// </summary>
        public FixedWidthReportItem ( )
        { } // public FixedWidthReportItem constructor (1 of 2)
        
        public FixedWidthReportItem (
            int pintColumnPosition ,
            string pstrPropertyName ,
            string pstrLabel )
        {
            this.ColumnPosition = pintColumnPosition;
            this.ValuePropertyName = pstrPropertyName;
            this.Label = pstrLabel;
        }   // public FixedWidthReportItem constructor (2 of 2)
        #endregion  // Constructors


        #region Private Storage for Instance
        int _intColumnPosition = INVALID_POSITION;
        string _strValuePropertyName = null;
        System.Reflection.PropertyInfo _propertyinfo = null;
        string _strLabel = null;
        int _intWidth = INVALID_WIDTH;
        #endregion  // Private Storage for Instance


        #region Properties
        /// <summary>
        /// Gets or sets the order of appearance of this field in the report.
        /// </summary>
        public int ColumnPosition
        {
            get { return _intColumnPosition; }
            set
            {
                const string ARGNAME = @"ColumnPosition Value";

                if ( value > INVALID_POSITION )
                {
                    _intColumnPosition = value;
                }   // TRUE (expected outcome) block, if ( value > INVALID_POSITION )
                else
                {
                    throw new ArgumentOutOfRangeException (
                        ARGNAME ,
                        value ,
                        COL_POS_REQUIREMENTS );
                }   // FALSE (UNexpected outcome) block, if ( value > INVALID_POSITION )
            }   // public int ColumnPosition property setter
        }   // public int ColumnPosition property (Read/Write)


        /// <summary>
        /// Gets or sets the label to display on the report.
        /// </summary>
        public string Label
        {
            get { return _strLabel; }
            set
            {
                _strLabel = EvaluateStringPropertyValue (
                    value ,
                    "Label property value" );
            }   // public string Label setter method
        }   // public string Label (Read/Write)


        /// <summary>
        /// Gets or sets the name of the property that supplies the values.
        /// </summary>
        /// <remarks>
        /// This property records a string representation of the name of the
        /// property that supplies the values. This class and its container
        /// class, FixedWidthReportItemsCollection, uses this name and the
        /// System.Reflection routines to gain access to the property value, to
        /// compute its width, and to render it on the report.
        /// </remarks>
        public string ValuePropertyName
        {
            get { return _strValuePropertyName; }
            set
            {
                _strValuePropertyName = EvaluateStringPropertyValue (
                    value ,
                    "ValuePropertyName property value" );
            }   // public string ValuePropertyName setter method
        }   // public string ValuePropertyName (Read/Write)


        /// <summary>
        /// Gets the minimum number of characters required to acommodate the
        /// column's label and values. This value is computed by evaluating the
        /// length of the label and the values in the underlying collection.
        /// </summary>
        public int Width
        {
            get
            {
                return _strLabel.Length > _intWidth ?
                    _strLabel.Length :
                    _intWidth;
            }   // public int Width getter method
        }   // public int Width (READ ONLY)
        #endregion  // Properties


        #region Instance Methods
        /// <summary>
        /// Return a fixed width label. This could be done in the 
        /// FixedWidthReportItemsCollection class, at the expense of two
        /// property access calls into this class. One of the two is
        /// unavoidable, except by duplicating some code in this method. I'll
        /// take the call overhead over the extra code to maintain.
        /// </summary>
        /// <returns>
        /// The return value is the Label property, padded on the right with
        /// spaces. You get spaces, period. This is one rare place where I won't
        /// give you a choice.
        /// </returns>
        public string CreateFixedWidthLabel ( )
        {
            return _strLabel.PadRight ( Width );
        }   // public string CreateFixedWidthLabel ( )


        /// <summary>
        /// Call this method to create a label guide to place beneath the label,
        /// to indicate the maximum width of the corresponding data field.
        /// </summary>
        /// <param name="pchrUseThis">
        /// Generate the label guide from this character. To use the default,
        /// LBL_GUIDE_DEFAULT, you may either pass it, or pass in a null
        /// character, LBL_GUIDE_USE_DEFAULT.
        /// </param>
        /// <returns>
        /// The return value is a string containing the number of characters
        /// that will be occupied by the label and its values.
        /// </returns>
        public string CreateLabelGuide ( char pchrUseThis )
        {
            if ( pchrUseThis == LBL_GUIDE_USE_DEFAULT )
                return string.Empty.PadRight (
                    Width ,
                    LBL_GUIDE_DEFAULT );
            else
                return string.Empty.PadRight (
                    Width ,
                    pchrUseThis );
        }   // public string CreateLabelGuide


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">
        /// This method accetps ANY type of object, since it's up to the named
        /// field to be able to render a string representation of itself.
        /// </typeparam>
        /// <param name="pTColumSource">
        /// Specify the instance to evaluate against the current value of the
        /// Width property.
        /// </param>
        /// <returns>
        /// The return value is a string representation of the field (property)
        /// of object pTColumSource.
        /// </returns>
        /// <remarks>
        /// This method leverages knowledge obtained while processing the set of
        /// objects in a first pass and creating the label row of the report.
        /// </remarks>
        public string GetFieldValue<T> ( T pTColumSource )
        {
            return _propertyinfo.GetValue ( pTColumSource , null ).ToString ( ).PadRight ( Width );
        }   // public string [ ] GetFieldValue


        /// <summary>
        /// Update the Width property to ensure that it is at least equal to the
        /// width of the current field value.
        /// </summary>
        /// <param name="pintFieldWidth">
        /// Compute the number of characters that will be occupied by the value
        /// of a field, and pass it into this method.
        /// </param>
        public void UpdateWidth ( int pintFieldWidth )
        {
            if ( pintFieldWidth > _strLabel.Length )
                if ( pintFieldWidth> _intWidth)
                    _intWidth = pintFieldWidth;
        }   // public void UpdateWidth ( int pintFieldWidth ) (1 of 2)


        /// <summary>
        /// Update the Width property to ensure that it is at least equal to the
        /// width of the current field value.
        /// </summary>
        /// <typeparam name="T">
        /// This method accetps ANY type of object, since it's up to the named
        /// field to be able to render a string representation of itself.
        /// </typeparam>
        /// <param name="pTColumSource">
        /// Specify the instance to evaluate against the current value of the
        /// Width property.
        /// </param>
        public void UpdateWidth<T> ( T pTColumSource )
        {
            if ( pTColumSource != null )
            {   // Null values can't participate.
                if ( _propertyinfo == null )
                {   // Using reflection is expensive; do this once only.
                    _propertyinfo = pTColumSource.GetType ( ).GetProperty ( _strValuePropertyName );
                }   // if ( _propertyinfo == null )

                object objValue = _propertyinfo.GetValue (
                    pTColumSource ,
                    null );
                UpdateWidth ( objValue.ToString ( ).Length );
            }   // if ( pTColumSource != null )
        }   // public void UpdateWidth<T> ( T pTColumSource ) (2 of 2)
        #endregion  // Instance Methods


        #region Overridden Methods of Base Class
        public override bool Equals ( object obj )
        {
            Type typOfComparand = obj.GetType ( );

            if ( typOfComparand == s_typOfMe )
            {   // Explicitly cast obj to my type, FixedWidthReportItem, so we can read its private storage.
                FixedWidthReportItem comparand = ( FixedWidthReportItem ) obj;
                return _intColumnPosition.Equals ( comparand._intColumnPosition );
            }   // TRUE (expected outcome) block, if ( typOfComparand == typOfMe )
            else
            {   // A type mismatch (BASIC error 13) has occurred.
                string strMag = string.Format (
                    ERRMSG_INCOMPARABLE ,
                    s_typOfMe ,
                    typOfComparand ,
                    Environment.NewLine );
                throw new InvalidCastException ( strMag );
            }   // FALSE (UNexpected outcome) block, if ( typOfComparand == typOfMe )
        }   // public override bool Equals ( object obj )


        public override int GetHashCode ( )
        {
            return _intColumnPosition.GetHashCode ( );
        }   // public override int GetHashCode ( )


        public override string ToString ( )
        {   //  Display something more useful than the object name. The Labels
            //  can be used to visually verify that they are ordered correctly.
            return _strLabel;
        }   // public override string ToString
        #endregion  // Overridden Methods of Base Class


        #region Private Static Utility Methods
        /// <summary>
        /// Evaluate a string property, returning its, so that the caller can
        /// save it, unless the value is a null reference (Nothing in Visual
        /// Basic) or the empty string, neither of which is allowed.
        /// </summary>
        /// <param name="pstrProposedValue">
        /// Specify the string to evaluate.
        /// </param>
        /// <param name="pstrPropertyName">
        /// Specify the property name to pass to the ArgumentNullException if
        /// pstrProposedValue is either a null reference or the empty string.
        /// </param>
        /// <returns>
        /// If the function succeeds, pstrProposedValue is returned, so that the
        /// caller can save it. Otherwise, an ArgumentNullException exception is
        /// thrown.
        /// </returns>
        private string EvaluateStringPropertyValue (
            string pstrProposedValue ,
            string pstrPropertyName )
        {
            if ( string.IsNullOrEmpty ( pstrProposedValue ) )
                throw new ArgumentNullException (
                    pstrPropertyName ,
                    string.Format (
                        LABEL_REQUIREMENTS ,
                        pstrPropertyName ) );
            else
                return pstrProposedValue;
        }   // if ( string.IsNullOrEmpty ( pstrProposedValue ) )
        #endregion  // Private Static Utility Methods


        #region IComparable Members
        public int CompareTo ( object obj )
        {
            Type typOfComparand = obj.GetType ( );

            if ( typOfComparand == s_typOfMe )
            {   // Explicitly cast obj to my type, FixedWidthReportItem, so we can read its private storage.
                FixedWidthReportItem comparand = ( FixedWidthReportItem ) obj;
                return _intColumnPosition.CompareTo ( comparand._intColumnPosition );
            }   // TRUE (expected outcome) block, if ( typOfComparand == typOfMe )
            else
            {   // A type mismatch (BASIC error 13) has occurred.
                string strMag = string.Format (
                    ERRMSG_INCOMPARABLE ,
                    s_typOfMe ,
                    typOfComparand ,
                    Environment.NewLine );
                throw new InvalidCastException ( strMag );
            }   // FALSE (UNexpected outcome) block, if ( typOfComparand == typOfMe )
        }   // public int CompareTo
        #endregion  // IComparable Members
    }   // class FixedWidthReportItem
}   // partial namespace TimeZoneLab