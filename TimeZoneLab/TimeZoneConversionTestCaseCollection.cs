/*
    ============================================================================

    Namespace:          TimeZoneLab

    Class Name:         TimeZoneConversionTestCaseCollection

    File Name:          TimeZoneConversionTestCaseCollection.cs

    Synopsis:           This class exposes a collection of test cases, in the
                        form of a generic List.

    Remarks:            The one and only constructor loads the collection from a
                        text file.

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

    Created:            Sunday, 31 August 2014 - Tuesday, 02 September 2014

    ----------------------------------------------------------------------------
    Revision History
    ----------------------------------------------------------------------------

    Date       Version Author Description
    ---------- ------- ------ --------------------------------------------------
    2014/09/02 1.0     DAG    Initial implementation.

	2016/06/14 2.0     DAG    1) Embed my three-clause BSD license, and improve
                                 the internal documentation.
    
							  2) Break dependence upon the deprecated SharedUtl2
                                 and ApplicationHelpers2 libraries.

	2021/10/10 3.0     DAG    1) Implement time zome abbreviation generation.

							  2) Upgrade to current libraries via NuGet.
    ============================================================================
*/


using System;
using System.Collections.Generic;
using System.IO;

using WizardWrx;


namespace TimeZoneLab
{
    class TimeZoneConversionTestCaseCollection : List<TimeZoneConversionTestCase>
    {
        //  For the moment, these are the actual property names.
        public const string COL_LBL_CASENUMBER = @"DisplayCaseNumber";                      // Format Item 0
        public const string COL_LBL_COMMENT = @"Comment";                                   // Format Item 1
        public const string COL_LBL_TESTDATE = @"DisplayTestDate";                          // Format Item 2
        public const string COL_LBL_TESTDATETIMEZONE = @"DisplayTestDateTimeZone";          // Format Item 3
        public const string COL_LBL_OUTPUTDATE = @"DisplayOutputDate";                      // Format Item 4
        public const string COL_LBL_OUTPUTDATETIMEZONE = @"DisplayOutputDateTimeZone";      // Format Item 5

        public const string ERRMSG_BAD_FILE = @"APPLICATION ERROR: The edge case file is invalid, because it is either empty or missing the required label row.{1}                   File name = {0}";
        public const string ERRMSG_BAD_RECORD = @"APPLICATION ERROR: The edge case file contains a bad record{5}                   File name            = {0}{5}                   Record Number        = {1}{5}                   Record Contents      = {2}{5}                   Actual Field Count   = {3}{5}                   Expected Field Count = {4}";

        public const int MINIMUM_RECORD_COUNT = 1;
        public const int POS_TEST_DATE = WizardWrx.ArrayInfo.ARRAY_FIRST_ELEMENT;
        public const int POS_TEST_COMMENT = POS_TEST_DATE + 1;
        public const int EXPECTED_FIELD_COUNT = POS_TEST_COMMENT + 1;

        public const string REPORT_FORMAT = @"{0} {1} {2} {3} {4} {5}";

        public const string TZ_ID_CENTRAL = @"Central Standard Time";
        public const string TZ_ID_MOUNTAIN = @"Mountain Standard Time";

        static string [ ] s_astrColLbls =
        {
            COL_LBL_CASENUMBER ,                                                // Format Item 0
            COL_LBL_COMMENT ,                                                   // Format Item 1
            COL_LBL_TESTDATE ,                                                  // Format Item 2
            COL_LBL_TESTDATETIMEZONE ,                                          // Format Item 3
            COL_LBL_OUTPUTDATE ,                                                // Format Item 4
            COL_LBL_OUTPUTDATETIMEZONE                                          // Format Item 5
        };  // static string [ ] s_astrColLbls

        TimeZoneInfo _tzTestValue = TimeZoneInfo.FindSystemTimeZoneById ( TZ_ID_MOUNTAIN );
        TimeZoneInfo _tzConverted = TimeZoneInfo.FindSystemTimeZoneById ( TZ_ID_CENTRAL );

        FixedWidthReportItemsCollection _reportitems = null;


        public FixedWidthReportItemsCollection ReportItems
        {
            get { return _reportitems; }
        }   // public FixedWidthReportItemsCollection ReportItems (READ ONLY and immutable)


        public TimeZoneInfo TZTestValue
        {
            get { return _tzTestValue; }
        }   // public TimeZoneInfo TestValue (READ ONLY and immutable)


        public TimeZoneInfo TZConverted
        {
            get { return _tzConverted; }
        }   // public TimeZoneInfo TZConverted (READ ONLY and immutable)


        public TimeZoneConversionTestCaseCollection ( )
        {
            const string ERRMSG_FNF = @"APPLICATION ERROR: The edge case file cannot be found.{1}                   File name = {0}";
            const string VALID_LABEL_ROW = @"TestDate	Comment";               // I left the embedded TAB.
                       
            string strTestCaseFileName = Properties.Settings.Default.EdgeCaseInputFileName;

            if ( File.Exists ( strTestCaseFileName ) )
            {
                string [ ] astrTestCases = File.ReadAllLines ( strTestCaseFileName );

                uint uintNCases = ( uint ) astrTestCases.Length;

                if ( uintNCases > MINIMUM_RECORD_COUNT )
                {
                    if ( astrTestCases [ ArrayInfo.ARRAY_FIRST_ELEMENT ] == VALID_LABEL_ROW )
                    {
                        for ( uint uintCastNumber = MINIMUM_RECORD_COUNT ;
                                   uintCastNumber < uintNCases ;
                                   uintCastNumber++ )
                        {
                            string [ ] astrFields = astrTestCases [ uintCastNumber ].Split ( SpecialCharacters.TAB_CHAR );

                            int intFieldCount = astrFields.Length;

                            if ( intFieldCount == EXPECTED_FIELD_COUNT )
                            {
                                TimeZoneConversionTestCase tc = new TimeZoneConversionTestCase (
                                    uintCastNumber ,                            // uint puintCaseNumber
                                    astrFields [ POS_TEST_DATE ] ,              // string pstrTestDate
                                    astrFields [ POS_TEST_COMMENT ] ,           // string pstrComment
                                    TZ_ID_MOUNTAIN ,                            // string pstrTZIDIn
                                    TZ_ID_CENTRAL );                            // string pstrTZIdOut
                                base.Add ( tc );
                            }   // TRUE (expected outcome) block, if ( intFieldCount == EXPECTED_FIELD_COUNT )
                            else
                            {   // The record is corrupted; it contains either too few fields or too many.
                                string strMsg = string.Format (
                                    ERRMSG_BAD_RECORD ,                         // Message template
                                    new string [ ]
                                    {
                                        strTestCaseFileName ,                   // Format Item 0
                                        uintCastNumber.ToString ( ) ,           // Format Item 1
                                        astrTestCases [ uintCastNumber ] ,      // Format Item 2
                                        intFieldCount.ToString ( ) ,            // Format Item 3
                                        EXPECTED_FIELD_COUNT.ToString ( ) ,     // Format Item 4
                                        Environment.NewLine                     // Format Item 5
                                    } );
                                throw new ApplicationException ( strMsg );
                            }   // FALSE (UNexpected outcome) block, if ( intFieldCount == EXPECTED_FIELD_COUNT )
                        }   // for ( uint uintCastNumber = MINIMUM_RECORD_COUNT ; uintCastNumber < uintNCases ; uintCastNumber++ )
                    }   // TRUE (expected outcome) block, if ( astrTestCases [ ArrayInfo.ARRAY_FIRST_ELEMENT ] == VALID_LABEL_ROW )
                    else
                    {   // The label row is invalid.
                        string strMsg = string.Format (
                            ERRMSG_BAD_FILE ,
                            strTestCaseFileName ,
                            Environment.NewLine );
                        throw new ApplicationException ( strMsg );
                    }   // FALSE (UNexpected outcome) block, if ( astrTestCases [ ArrayInfo.ARRAY_FIRST_ELEMENT ] == VALID_LABEL_ROW )
                }   // TRUE (expected outcome) block, if ( astrTestCases.Length > MINIMUM_RECORD_COUNT )
                else
                {   // The file contains zero or 1 lines, enough, at most, for a label record.
                    string strMsg = string.Format (
                        ERRMSG_BAD_FILE ,
                        strTestCaseFileName ,
                        Environment.NewLine );
                    throw new ApplicationException ( strMsg );
                }   // FALSE (UNexpected outcome) block, if ( astrTestCases.Length > MINIMUM_RECORD_COUNT )

                _reportitems = new FixedWidthReportItemsCollection (
                    s_astrColLbls ,                                             // string [ ] pastrlOrderedLabels
                    Properties.Settings.Default.EdgeCaseReportFileName );       // string pstrOutputFileName
            }   // TRUE (expected outcome) block, if ( File.Exists ( strTestCaseFileName ) )
            else
            {   // The specified file of test cases cannot be found.
                string strMsg = string.Format (
                    ERRMSG_FNF ,
                    strTestCaseFileName ,
                    Environment.NewLine );
                throw new ApplicationException ( strMsg );
            }   // FALSE (UNexpected outcome) block, if ( File.Exists ( strTestCaseFileName ) )
        }   // public TimeZoneConversionTestCaseCollection constructor (1 of 1)


        public void CreateReport ( )
        {
            //  ----------------------------------------------------------------
            //  First things first - compute the required field widths.
            //  ----------------------------------------------------------------

            foreach ( TimeZoneConversionTestCase tc in this )
            {
                _reportitems.UpdateColumnWidths ( tc );
            }   // foreach ( TimeZoneConversionTestCase tc in this )

            //  ----------------------------------------------------------------
            //  Since it depends entirely on information known to _reportitems,
            //  it requires only a single method call.
            //  FixedWidthReportItemsCollection.
            //  ----------------------------------------------------------------

            _reportitems.CreateReportHeading ( );

            //  ----------------------------------------------------------------
            //  Writing the report requires another pass through the collection.
            //  ----------------------------------------------------------------

            foreach ( TimeZoneConversionTestCase tc in this )
            {
                _reportitems.CreateReportRecord ( tc );
            }   // foreach ( TimeZoneConversionTestCase tc in this )

            //  ----------------------------------------------------------------
            //  If the report went to a file, there is a StreamWriter to close.
            //  ----------------------------------------------------------------

            _reportitems.CloseReport ( );
        }  // public void CreateReport
    }   // class CreateReportHeading
}   // partial namespace TimeZoneLab