﻿using Dfc.ProviderPortal.Apprenticeships.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        public IActionResult AddApprenticeship([Required]string code)
        {
            return Ok();
        }

        [Route("GetApprenticeshipByUKPRN")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Apprenticeship>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetApprenticeshipByUKPRN(string UKPRN, [Required]string code)
        {
            return Ok();
        }

        [Route("SearchApprenticeshipById")]
        [HttpGet]
        [ProducesResponseType(typeof(Apprenticeship), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SearchApprenticeshipById(Guid id, [Required]string code)
        {
            return Ok();
        }

        [Route("StandardsAndFrameworksSearch")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StandardsAndFrameworks>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult StandardsAndFrameworksSearch(string search, [Required]string code)
        {
            return Ok();
        }

        [Route("UpdateApprenticeship")]
        [HttpPost]
        [ProducesResponseType(typeof(Apprenticeship), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateApprenticeship(Apprenticeship apprenticeship, [Required]string code)
        {
            return Ok();
        }
    }
}