// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

// Original file: https://github.com/DuendeSoftware/IdentityServer.Quickstart.UI
// Modified by Jan Škoruba

using OpenVision.IdentityServer.STS.Identity.ViewModels.Consent;

namespace OpenVision.IdentityServer.STS.Identity.ViewModels.Device;

public class DeviceAuthorizationViewModel : ConsentViewModel
{
    public string UserCode { get; set; }
    public bool ConfirmUserCode { get; set; }
}
