# Deploy to nuget.com

1. Change version information in AssemblyInfo.cs using `assembly: AssemblyInformationalVersion`

     Samples:

     1.1.1 - production build number

     1.1.1-beta - test builds

2. Merge to git repository

    2.1. Merge to `dev` for test builds (assign a suffix to the version, e.g. -beta)

    2.2. Merge to `master` for production builds (remove any suffix)

3. visualstudio.com will automatically compile and deploy to nuget.com