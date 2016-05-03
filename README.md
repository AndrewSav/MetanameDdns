MetanameDdns
============
This is a Dynamic DNS update client and .NET API wrapper for http://metaname.net/
This solution consists of two parts:
  - [Metaname API][1] .Net Wrapper
  - Dynamic DNS Update Client for Metaname using their API
  
**Installation**
 - Copy the binaries and the configs in a folder
 - Edit `config.json` to suit your needs
 - Ininitially you can leave `logconfig.xml` as is, but once you get up and running you might want to fine tune the amount of log output. See [Apache log4net™ Manual - Configuration][2] to find out how to configure log4net
 - Be an administrator and *run as administrator* `MetanameDdns.exe`. I suggest running  `cmd.exe` or `powershell.exe` as administrator and then running `MetanameDdns.exe` from there to be able to see the console output.
 - The windows service will get installed and started. It's installed to run as LocalSystem, if you need to change that run `services.msc` and chanage it the usual way.
 - Examine the log file (by default it's `metaname-ddns.log`) to make sure there is no problem 
 - You can uninstall the service with `MetanameDdns.exe /uninstall` or run as an executable with `MetanameDdns.exe /console`

**Compilation**

I used Visual Studio 2013, but I have no reason to think that it would not work in Visual Studion 2012 as well. I have not checked in the nuget packages in git, so you need to make sure that the package restore is enabled in your Visual Studio.
 
**Configuration and API**

I like to beleive that the configuration file and the API usage are self-explanatory. Examples of calling the API can be found in the MetanameDdns project. *Note* however, that while all documented (as of the time of writing) API is implelemented not all of it was properly tested, as I needed only two of them for the updater. Let me know if there are any issues.

**Dependencies**
 - Compiled with .NET Framework 4.5
 - [Newtonsoft Json.Net][3]
 - [log4net][4]
 - Some ideas are taken from [Jayrock][5]
 - Some ideas are take from [Route53Ddns][6]
 - Some code pieces are borrowed from awesome folk on [stackoverflow.com][7]

  [1]: https://metaname.net/api/1.1/doc
  [2]: http://logging.apache.org/log4net/release/manual/configuration.html
  [3]: http://james.newtonking.com/json
  [4]: http://logging.apache.org/log4net/
  [5]: https://atifaziz.github.io/projects/jayrock/
  [6]: https://github.com/dreamins/Route53DDNS
  [7]: http://stackoverflow.com
  
  
