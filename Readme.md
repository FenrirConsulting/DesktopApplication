# AD Admin Desktop Utility

## Overview
A .NET Core desktop application designed for Active Directory administrators. This utility provides a suite of tools for common AD management tasks, streamlining administrative workflows and enhancing productivity.

## Key Features
- 👥 User Account Management
  - Create, modify, and disable user accounts
  - Reset passwords and manage account policies
  - Bulk user operations
- 🏢 Group Management
  - Add/remove users from groups
  - Create and modify security groups
  - Group membership reporting
- 📊 AD Reporting Tools
  - Generate user and group reports
  - Track account statuses
  - Monitor group memberships
- 🔒 Security Features
  - Secure credential handling
  - Audit logging
  - Role-based access control

## Prerequisites
- Windows OS
- .NET Core 6.0 or higher
- Domain Admin or appropriate AD permissions
- Active Directory domain environment

## Installation
1. Download the latest release from the Releases page
2. Run the installer package
3. Launch the application
4. Authenticate with domain admin credentials

## Usage Guide

### User Management
```powershell
# Example PowerShell commands the application executes
New-ADUser -Name "John Doe" -SamAccountName "jdoe" -UserPrincipalName "jdoe@domain.com"
```

### Group Management
```powershell
# Example group management operations
Add-ADGroupMember -Identity "IT Department" -Members "jdoe"
```

### Reporting
- Generate user activity reports
- Export group membership lists
- Track account modifications

## Configuration
The application uses a configuration file (`appsettings.json`) for customizing:
- Domain settings
- Report output locations
- Logging preferences
- UI preferences

## Security Considerations
- Runs with user's AD credentials
- Implements least privilege access
- Logs all administrative actions
- Secures sensitive data in memory

## Troubleshooting
Common issues and solutions:
1. Connection Issues
   - Verify domain connectivity
   - Check credentials
   - Ensure proper DNS resolution
2. Permission Issues
   - Verify AD admin rights
   - Check group memberships
   - Review security policies

## Building from Source
```bash
# Clone the repository
git clone https://github.com/FenrirConsulting/DesktopApplication.git

# Navigate to project directory
cd DesktopApplication

# Build the solution
dotnet build

# Run the application
dotnet run
```

## Contributing
Contributions are welcome! Please read our contributing guidelines before submitting pull requests.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support
For support:
1. Check the [Issues](https://github.com/FenrirConsulting/DesktopApplication/issues) page
2. Submit a new issue for bugs or feature requests
3. Review existing documentation

## Author
Christopher Olson - Senior Security Engineer

## Acknowledgments
- Microsoft Active Directory Team
- .NET Core Community
- Contributors and testers