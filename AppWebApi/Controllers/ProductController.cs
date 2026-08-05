using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Services;

namespace AppWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{


    readonly IProductService _service;
    readonly ILogger<ProductController> _logger;

    public ProductController(IProductService service, ILogger<ProductController> logger)
    {
        _service = service;
        _logger = logger;

    }

    [HttpGet]
    [ProducesResponseType(typeof(ResponsePageDto<ProductListDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 0, [FromQuery] int pageSize = 10)
    {
        var result = await _service.ReadProductsAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ResponseItemDto<ProductDetailDto>), 200)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.ReadProductAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(ResponseItemDto<ProductDetailDto>), 201)]
    public async Task<IActionResult> Create([FromBody] ProductCreateRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await _service.CreateProductAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Item.ProductId }, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ResponseItemDto<ProductDetailDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductUpdateRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var result = await _service.UpdateProductAsync(id, request);
        return result == null ? NotFound() : Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteProductAsync(id);
        return result ? NoContent() : NotFound();
    }
}
