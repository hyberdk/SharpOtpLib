

	This is #OTP Lib (Sharp One-Time Password Library)

	1. Introduction
	2. How to use
	3. Releases
	4. Roadmap
	5. The Author


Introduction
============

	#OTP Lib is an implementation of the RFC4226 (HOTP: An HMAC-Based One-Time
	Password Algorithm) and RFC6238 (TOTP: Time-Based One-Time Password 
	Algorithm) to provide a simple two-factor authentication written in C#.
	You can use this library to implement your own One-Time Passwords if you 
	want two-factor authentication for your systems and then use the already 
	build Google Authenticator as the client soft-token.
	
	You can of cause also use any compatible HOTP/TOTP software or hardware token.
	As said #OTP lib is compatible with Google Authenticator and other OATH 
	implementations.
	Google Authenticator (http://code.google.com/p/google-authenticator/) 
	is available for
	  * Android: https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2
	  * iPhone: http://itunes.apple.com/us/app/google-authenticator/id388497605?mt=8
	  * Blackbarry

How to use
==========

	A. HOTP example - HMAC-based One-Time Password Example
	------------------------------------------------------

	// this is a simple HOTP use test (that will fail due to wrong code)
        private void test()
        {
            HashbasedOneTimePassword hotp = new HashbasedOneTimePassword();
            byte[] key = Utils.GenerateSecret();
            string ident = "hotp-test";
            hotp.Devices.Add(ident, key);
            OneTimePassword.VerifyResult result = hotp.Verify(ident, "123456");
        }
	
	B. TOTP - Time-based One-Time Password Example
	----------------------------------------------

	// this is a simple TOTP use test (that will fail due to wrong code)
        private void test()
        {
            TimebasedOneTimePassword hotp = new TimebasedOneTimePassword();
            byte[] key = Utils.GenerateSecret();
            string ident = "totp-test";
            totp.Devices.Add(ident, key);
            OneTimePassword.VerifyResult result = totp.Verify(ident, "123456");
        }

		
Releases
========

	A. Downlaod
	-----------
	
	You can download the binaries / documentation here
	 * Current Release (v1.0.0)
	    - Binaries https://dl.dropbox.com/u/2752232/SharpOtpLib/SharpOtpLib_Binaries_v1.0.zip
	    - Documentation: https://dl.dropbox.com/u/2752232/SharpOtpLib/SharpOtpLib_Documentation_v1.0.zip
	    - Source Zip: https://dl.dropbox.com/u/2752232/SharpOtpLib/SharpOtpLib_Source_v1.0.zip
	    - Source Git: https://github.com/hyberdk/SharpOtpLib
	
	B. Changelog
	------------
	
	 * v1.0.0 - Initial release


Road Map
========

	None yet. Perhaps make compliant with OATH specs. Send me your thoughts.

	
	
The Author
==========

	This software was written and is currently maintained by me Esben Laursen, 
	hyber@hyber.dk, Phone +45 51940654, I write this code because I find it 
	challenging and I needed a two-factor authentication for my VPN at home. 
	Furthermore I am a big believer in OpenSource and I believe that it is 
	important to give something back.
	You are very welcome to drop me an email if you find my project interesting
	or if you find bug. For help, I will try and help as I can, but please keep 
	in mind, this is mainly for my amusement and I have no finicial grain from 
	this, so I have to maintain a day job (and keep the wife happy) so my time 
	is limited.



License
=======

	#OTP is licensed under LGPL (GNU Lesser General Public License), 
	see http://www.gnu.org/copyleft/lesser.html for details about the license.
	It means that you can use this library in proprietary software without 
	opening your source-code. If you feel that you this license is not the best 
	for you, please contact me: hyber@hyber.dk
