using API.DTOs;
using API.Errors;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
   
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<ProductBrand> _productsBrandRepo;
        private readonly IGenericRepository<ProductType> _productTypeRepo;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productsRepo, 
                                  IGenericRepository<ProductBrand> productsBrandRepo, 
                                  IGenericRepository<ProductType> productTypeRepo,
                                  IMapper mapper)
        {

            _productsRepo = productsRepo;
            _productsBrandRepo = productsBrandRepo;
            _productTypeRepo = productTypeRepo;
            _mapper = mapper;
        }

        [HttpGet]

        public async Task <ActionResult<IReadOnlyList<ProductToReturnUrl>>> GetProducts()
        {
            var spec = new ProductsWithTypesAndBrandsSpecification();

            var products = await _productsRepo.ListAsync(spec);

            return Ok(_mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnUrl>>(products));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),(StatusCodes.Status404NotFound))]
        public async Task<ActionResult<ProductToReturnUrl>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);
            
            var product = await _productsRepo.GetEntityWithSpec(spec);

            if (product == null) return NotFound(new ApiResponse(404));

            return _mapper.Map<Product, ProductToReturnUrl>(product);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok (await _productsBrandRepo.ListAllSync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            return Ok (await _productTypeRepo.ListAllSync());
        }
    }
}
