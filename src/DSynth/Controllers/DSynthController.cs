/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
#nullable enable

using System.Collections.Generic;
using System.Threading.Tasks;
using DSynth.Common.Options;
using DSynth.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DSynth.Models;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace DSynth.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DSynthController : ControllerBase
    {
        private readonly IDSynthService _dSynthService;
        private readonly IProfileService _profileService;
        private readonly TelemetryClient _telemetryClient;
        private readonly ILogger<DSynthController> _logger;

        public DSynthController(
            IDSynthService dSynthService,
            IProfileService profileService,
            TelemetryClient telemetryClient,
            ILogger<DSynthController> logger)
        {
            _dSynthService = dSynthService;
            _profileService = profileService;
            _telemetryClient = telemetryClient;
            _logger = logger;
        }

        /// <summary>
        /// Stops DSynth
        /// </summary>
        [HttpGet]
        [Route("Stop")]
        public async Task<IActionResult> Stop()
        {
            await _dSynthService.StopAsync().ConfigureAwait(false);
            _telemetryClient.TrackEvent(new EventTelemetry("DSynthStop"));

            var endpointResp = EndpointResponse.CreateNew(200, "Stopped DSynth successfully!");
            return endpointResp.Resp;
        }

        /// <summary>
        /// Starts DSynth
        /// </summary>
        [HttpGet]
        [Route("Start")]
        public async Task<IActionResult> Start()
        {
            try
            {
                await _dSynthService.StartAsync().ConfigureAwait(false);
                _telemetryClient.TrackEvent(new EventTelemetry("DSynthStart"));

                var endpointResp = EndpointResponse.CreateNew(200, "Started DSynth successfully!");
                return endpointResp.Resp;
            }
            catch (DSynthServiceException ex)
            {
                var endpointResp = EndpointResponse.CreateNew(500, ex.Message);
                return endpointResp.Resp;
            }
        }

        /// <summary>
        /// Restarts DSynth
        /// </summary>
        [HttpGet]
        [Route("Restart")]
        public async Task<IActionResult> Restart()
        {
            try
            {
                await _dSynthService.RestartAsync().ConfigureAwait(false);
                _telemetryClient.TrackEvent(new EventTelemetry("DSynthRestart"));

                var endpointResp = EndpointResponse.CreateNew(200, "Restarted DSynth successfully!");
                return endpointResp.Resp;
            }
            catch (DSynthServiceException ex)
            {
                var endpointResp = EndpointResponse.CreateNew(500, ex.Message);
                return endpointResp.Resp;
            }
        }

        /// <summary>
        /// Retrieves the currently running provider options
        /// </summary>
        [HttpGet]
        [Route("ProvidersOptions")]
        public IActionResult ProvidersOptions()
        {
            return StatusCode(200, _dSynthService.GetProvidersOptions());
        }

        /// <summary>
        /// Updates the currently running provider options
        /// </summary>
        [HttpPost]
        [Route("ProviderOptions")]
        public async Task<IActionResult> ProvidersOptions(List<DSynthProviderOptions> providersOptions)
        {
            // TODO: Need to handle exceptions
            var updatedProviderOptions = await _dSynthService.UpdateProvidersOptionsAsync(providersOptions)
                .ConfigureAwait(false);

            return StatusCode(200, updatedProviderOptions);
        }

        /// <summary>
        /// Gets the available profiles that can be activated
        /// </summary>
        [HttpGet]
        [Route("Profiles")]
        public IActionResult Profiles()
        {
            try
            {
                return StatusCode(200, _profileService.GetAvailableProfiles());
            }
            catch (DSynthServiceException ex)
            {
                var endpointResp = EndpointResponse.CreateNew(500, ex.Message);
                return endpointResp.Resp;
            }
        }

        /// <summary>
        /// Imports packaged profile[s]
        /// </summary>
        [HttpPost]
        [Route("Profiles/Import")]
        public async Task<IActionResult> ImportProfiles(IFormFile file)
        {
            try
            {
                var foundEntries = await _profileService.ImportProfilesPackageAsync(file).ConfigureAwait(false);

                var endpointResp = EndpointResponse.CreateNew(200, $"Imported profiles successfully! Found the following entries in the zip file: '{foundEntries}");
                return endpointResp.Resp;
            }
            catch (ProfileServiceException ex)
            {
                var endpointResp = EndpointResponse.CreateNew(500, ex.Message);
                return endpointResp.Resp;
            }
        }

        /// <summary>
        /// Exports all profiles as a package
        /// </summary>
        [HttpGet]
        [Route("Profiles/Export")]
        public IActionResult ExportProfiles()
        {
            try
            {
                var byteData = _profileService.ExportProfilesPackage();
                return File(byteData, "application/octet-stream", Resources.ProfileService.ProfilesPackageName);
            }
            catch (ProfileServiceException ex)
            {
                var endpointResp = EndpointResponse.CreateNew(500, ex.Message);
                return endpointResp.Resp;
            }
        }

        /// <summary>
        /// Activates a given profile
        /// </summary>
        [HttpPost]
        [Route("Profiles/Activate/{profileName}")]
        public async Task<IActionResult> ActivateProfile(string profileName)
        {
            try
            {
                await _profileService.ActivateProfile(profileName).ConfigureAwait(false);
                await _dSynthService.RestartAsync().ConfigureAwait(false);

                var endpointResp = EndpointResponse.CreateNew(200, $"Activated profile '{profileName}' successfully!");
                return endpointResp.Resp;
            }
            catch (ProfileServiceException ex)
            {
                var endpointResp = EndpointResponse.CreateNew(500, ex.Message);
                return endpointResp.Resp;
            }
        }

        /// <summary>
        /// Retrieves the status of DSynth
        /// </summary>
        [HttpGet]
        [Route("Status")]
        public IActionResult Status()
        {
            return Ok(_dSynthService.GetStatus());
        }

        /// <summary>
        /// Retrieves all providers that can be used with GetNextPayload
        /// </summary>
        [HttpGet]
        [Route("Providers")]
        public IActionResult ProviderNames()
        {
            return Ok(_dSynthService.GetProviderNames());
        }

        /// <summary>
        /// Retrieves a payload for a given provider
        /// </summary>
        [HttpGet]
        [HttpPost]
        [HttpPatch]
        [Route("Providers/{providerName}/GetNextPayload")]
        public IActionResult ProviderGetNextPayload(string providerName)
        {
            // Remove accept header because the output is determined by the provider
            if (Request.Headers.TryGetValue(HttpRequestHeader.Accept.ToString(), out _))
            {
                Request.Headers.Remove(HttpRequestHeader.Accept.ToString());
            }

            _logger.LogInformation(new ResponseLog(Request).ToString());

            return Ok(_dSynthService.GetNextPayload(providerName).PayloadAsString);
        }
    }

    public class ResponseLog
    {
        public ResponseLog(HttpRequest request)
        {
            using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                this.RequestBody = reader.ReadToEndAsync().Result;
            }

            this.RequestMethod = request.Method;
            this.RequestHeaders = request.Headers;
            this.RequestPath = request.Path;
            this.RequestQueryString = request.QueryString.ToString();
        }

        public string RequestPath { get; set; }
        public string RequestQueryString { get; }
        public string RequestMethod { get; set; }
        public IHeaderDictionary RequestHeaders { get; set; }
        public string? RequestBody { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}