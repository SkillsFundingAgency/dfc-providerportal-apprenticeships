using Dfc.ProviderPortal.Apprenticeships.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dfc.ProviderPortal.Apprenticeships.Functions;
using Dfc.ProviderPortal.Apprenticeships.Models.Tribal;

namespace Dfc.ProviderPortal.Apprenticeships.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    [ApiController]
    public class DocController : ControllerBase
    {
        [Route("AddApprenticeship")]
        [HttpPost]
        [ProducesResponseType(typeof(Apprenticeship), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddApprenticeship()
        {
            return Ok();
        }

        [Route("AddApprenticeshipMigrationReport")]
        [HttpPost]
        [ProducesResponseType(typeof(ApprenticeshipMigrationReport), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddApprenticeshipMigrationReport()
        {
            return Ok();
        }

        [Route("GetApprenticeshipByUKPRN")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Apprenticeship>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetApprenticeshipByUKPRN(string UKPRN)
        {
            return Ok();
        }

        [Route("GetStandardsAndFrameworksById")]
        [HttpGet]
        [ProducesResponseType(typeof(StandardsAndFrameworks), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetStandardsAndFrameworksById(Guid id, int type)
        {
            return Ok();
        }

        [Route("GetApprenticeshipById")]
        [HttpGet]
        [ProducesResponseType(typeof(Apprenticeship), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetApprenticeshipById(Guid id)
        {
            return Ok();
        }

        [Route("StandardsAndFrameworksSearch")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StandardsAndFrameworks>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult StandardsAndFrameworksSearch(string search, string UKPRN)
        {
            return Ok();
        }

        [Route("UpdateApprenticeship")]
        [HttpPost]
        [ProducesResponseType(typeof(Apprenticeship), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateApprenticeship(Apprenticeship apprenticeship)
        {
            return Ok();
        }

        [Route("ChangeApprenticeshipStatusForUKPRNSelection")]
        [HttpGet]
        [ProducesResponseType(typeof(ChangeApprenticeshipStatusForUKPRNSelection), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ChangeApprenticeshipStatusForUKPRNSelection(int UKPRN, int CurrentStatus, int StatusToBeChangedTo)
        {
            return Ok();
        }

        [Route("DeleteBulkUploadApprenticeships")]
        [HttpGet]
        [ProducesResponseType(typeof(DeleteBulkUploadApprenticeships), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteBulkUploadApprenticeships(int UKPRN)
        {
            return Ok();
        }

        [Route("DeleteApprenticeshipsByUKPRN")]
        [HttpPost]
        [ProducesResponseType(typeof(DeleteApprenticeshipsByUKPRN), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteApprenticeshipsByUKPRN(int UKPRN)
        {
            return Ok();
        }
        [Route("TribalGetAllApprenticeships")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TribalProvider>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult TribalGetAllApprenticeships()
        {
            return Ok();
        }
        [Route("TribalGetProviderByUKPRN")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TribalProvider>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult TribalGetProviderByUKPRN(string UKPRN)
        {
            return Ok();
        }
        [Route("GetUpdatedApprenticeships")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Apprenticeship>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetUpdatedApprenticeships()
        {
            return Ok();
        }
        [Route("GetUpdatedApprenticeshipsAsProvider")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TribalProvider>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetUpdatedApprenticeshipsAsProvider()
        {
            return Ok();
        }
        [Route("GetUpdatedApprenticeshipsAsProviderByUKPRN")]
        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<TribalProvider>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetUpdatedApprenticeshipsAsProviderByUKPRN(int UKPRN)
        {
            return Ok();
        }
        [Route("GetStandardByCode")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StandardsAndFrameworks>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetStandardByCode(string standardCode, string version)
        {
            return Ok();
        }
        [Route("GetFrameworkByCode")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StandardsAndFrameworks>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetFrameworkByCode(int frameworkCode, int progType, int pathwayCode)
        {
            return Ok();
        }
    }
}