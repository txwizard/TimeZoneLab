/*
    ============================================================================

    Namespace:          TimeZoneLab

    Class Name:         TZHelpers

    File Name:          TZHelpers.cs

    Synopsis:           This class exposes utility methods for working with the
                        time zone information stored in the Windows Registry.

    Remarks:            I can never remember from one time to the next whether
						the default scope of a class is private or internal; it
                        is internal, since other routines see this class as soon
						as I created it.

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

    Created:            Saturday, 30 August 2014 - Tuesday 02 September 2014

    ----------------------------------------------------------------------------
    Revision History
    ----------------------------------------------------------------------------

    Date       Version Author Synopsis
    ---------- ------- ------ -------------------------------------------------
	2016/06/14 2.0     DAG    This class is a slimmed-down version of the old
                              UTIL class, which I just discarded, thinking that
                              everything in it, except one constant, duplicates
                              material that is in WizardWrx.DLLServices2, which
                              is bound into the project for other substantial
                              reasons.

	2022/07/12 4.0     DAG    Implement routines to query the Windows Registry
                              for information that the TimeZoneInfo class hides.
    ============================================================================
*/


using System;

using WizardWrx;

namespace TimeZoneLab
{
	/// <summary>
	/// This is what's left of the old Util class.
	/// </summary>
	internal static class TZHelpers
	{
		/// <summary>
		/// Feed this to GetDisplayTimeZone to retrieve the TimeZoneInfo data
		/// that defines the Coordinated Universal Time (UTC) time zone.
		/// </summary>
		/// <remarks>
		/// Since it is locale-agnostic, the preferred method of retrieving the
		/// UTC time zone information is through the static TimeZoneInfo.Utc
		/// property.
		/// </remarks>
		public const string UTC_TIMEZONE_ID = @"UTC";


		/// <summary>
		/// Given a DateTime and a system time zone ID string, return the
		/// appropriate text to display, depending on whether the specified time
		/// is standard or Daylight Saving Time.
		/// </summary>
		/// <param name="pdtmTestDate">
		/// Specify the Syatem.DateTime for which the appropriate time zone
		/// string is required.
		/// </param>
		/// <param name="pstrTimeZoneID">
		/// Specify a valid time zone ID string. Please see the Remarks.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is the appropriate string
		/// to display for the given time. Otherwise, the empty string is
		/// returned or one of several exceptions is thrown, the most likely of
		/// which is a TimeZoneNotFoundException, which is thrown when the
		/// specified time zone ID string is invalid.
		/// </returns>
		/// <exception cref="OutOfMemoryException">
		/// You should restart Windows if this happens.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// Contact the author of the program. This is something that he or she
		/// must address.
		/// </exception>
		/// <exception cref="TimeZoneNotFoundException">
		/// Contact the author of the program. This is something that he or she
		/// must address.
		/// </exception>
		/// <exception cref="System.Security.SecurityException">
		/// Contact your system administrator to inquire about why your program
		/// is forbidden to read the regional settings from the Windows
		/// Registry.
		/// </exception>
		/// <exception cref="InvalidTimeZoneException">
		/// Contact your system support group. A corrupted Windows Registry is a
		/// rare, but serious matter.
		/// </exception>
		/// <exception cref="Exception">
		/// Start with your system support group, who may need to request the
		/// assistance of the author of the program.
		/// </exception>
		/// <remarks>
		/// if in doubt, use TimeZoneInfo.GetSystemTimeZones to enumerate the
		/// time zones installed on the local machine.
		/// </remarks>
		public static string GetDisplayTimeZone (
			DateTime pdtmTestDate ,
			string pstrTimeZoneID )
		{
			const string ERRMSG_NO_MEMORY = "SYSTEM RESOURCE FAMINE: The GetDisplayTimeZone method ran out of memory.";
			const string ERRMSG_NULL_TZ_ID = "INTERNAL ERROR: The GetDisplayTimeZone method let a null pstrTimeZoneID through to TimeZoneInfo.FindSystemTimeZoneById.";
			const string ERRMSG_TZ_NOT_FOUND = "INTERNAL or DATA ERROR: The GetDisplayTimeZone method let a pstrTimeZoneID that isn't registered on this computer through to TimeZoneInfo.FindSystemTimeZoneById.{1}                        Specified ID = {0}";
			const string ERRMSG_SECURITY = "ACCESS VIOLATION: The GetDisplayTimeZone method cannot read the Registry keys where the time zone information is kept. The process has insufficient access permissions on that key.";
			const string ERRMSG_INV_TZINFO = "CORRUPTED SYSTEM REGISTRY: The GetDisplayTimeZone method found the specified key, but the corresponding Registry key is corrupted.{1}                           Specified ID = {0}";
			const string ERRMSG_RUNTIME = "RUNTIME EXCEPTION: The GetDisplayTimeZone method found the specified key, but the corresponding Registry key is corrupted.{1}                    Specified ID = {0}";

			if ( pdtmTestDate == DateTime.MinValue || pdtmTestDate == DateTime.MaxValue || string.IsNullOrEmpty ( pstrTimeZoneID ) )
			{   // Insufficient data available
				return string.Empty;
			}   // TRUE (degenerate case) block, if ( pdtmTestDate == DateTime.MinValue || pdtmTestDate == DateTime.MaxValue || string.IsNullOrEmpty(pstrTimeZoneID) )
			else
			{
				try
				{
					TimeZoneInfo tzinfo = TimeZoneInfo.FindSystemTimeZoneById ( pstrTimeZoneID );
					return tzinfo.IsDaylightSavingTime ( pdtmTestDate ) ?
						tzinfo.DaylightName :
						tzinfo.StandardName;
				}
				catch ( OutOfMemoryException exNoMem )
				{
					throw new Exception (
						ERRMSG_NO_MEMORY ,
						exNoMem );
				}
				catch ( ArgumentNullException exNullID )
				{
					throw new Exception (
						ERRMSG_NULL_TZ_ID ,
						exNullID );
				}
				catch ( TimeZoneNotFoundException exTZNotFound )
				{
					throw new Exception (
						string.Format (
							ERRMSG_TZ_NOT_FOUND ,
							pstrTimeZoneID ,
							Environment.NewLine ) ,
						exTZNotFound );
				}
				catch ( System.Security.SecurityException exSecurity )
				{
					throw new Exception (
						ERRMSG_SECURITY ,
						exSecurity );
				}
				catch ( InvalidTimeZoneException exInvTZInfo )
				{
					throw new Exception (
						string.Format (
							ERRMSG_INV_TZINFO ,
							pstrTimeZoneID ,
							Environment.NewLine ) ,
						exInvTZInfo );
				}
				catch ( Exception exMisc )
				{
					throw new Exception (
						string.Format (
							ERRMSG_RUNTIME ,
							pstrTimeZoneID ,
							Environment.NewLine ) ,
						exMisc );
				}
			}   // FALSE (desired outcome) block, if ( pdtmTestDate == DateTime.MinValue || pdtmTestDate == DateTime.MaxValue || string.IsNullOrEmpty(pstrTimeZoneID) )
		}   // public static string GetDisplayTimeZone
	}   // internal static class TZHelpers
}	// partial namespace TimeZoneLab