using AutoMapper;
using GameServer.Core.GameAggregate;
using GameServer.Core.GameAggregate.Specifications;
using GameServer.SharedKernel.ApiModels;
using GameServer.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Web.Api;

public class ShopItemsController : BaseApiController
{
  private readonly IRepository<ShopItem> _shopItemRepository;
  private readonly IMapper _mapper;

  public ShopItemsController(IRepository<ShopItem> shopItemRepository, IMapper mapper)
  {
    _shopItemRepository = shopItemRepository;
    _mapper = mapper;
  }

  [Authorize]
  [HttpGet]
  public async Task<IActionResult> List()
  {
    try
    {
      var shopItems = (await _shopItemRepository.ListAsync()).Select(_mapper.Map<ShopItem, ShopItemDTO>);

      return Ok(shopItems);
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpPost]
  public async Task<IActionResult> Create(ShopItemDTO shopItemDto)
  {
    try
    {
      var shopItem = await _shopItemRepository.AddAsync(_mapper.Map<ShopItem>(shopItemDto));

      return Ok(_mapper.Map<ShopItemDTO>(shopItem));
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }

  [Authorize]
  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetById(int id)
  {
    try
    {
      var shopItem = await _shopItemRepository.FirstOrDefaultAsync(new ShopItemByIdSpec(id));
      if (shopItem == null) return NotFound();

      return Ok(_mapper.Map<ShopItemDTO>(shopItem));
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }

  [Authorize(Roles = "Admin")]
  [HttpPut]
  public async Task<IActionResult> Update(ShopItemDTO shopItemDto)
  {
    try
    {
      var shopItem = await _shopItemRepository.FirstOrDefaultAsync(new ShopItemByIdSpec(shopItemDto.Id));
      if (shopItem == null) return NotFound();

      _mapper.Map(shopItemDto, shopItem);
      await _shopItemRepository.UpdateAsync(shopItem);
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }

    return Ok();
  }

  [Authorize(Roles = "Admin")]
  [HttpDelete("{id:int}")]
  public async Task<IActionResult> Delete(int id)
  {
    try
    {
      var shopItem = await _shopItemRepository.FirstOrDefaultAsync(new ShopItemByIdSpec(id));
      if (shopItem == null) return NotFound();

      await _shopItemRepository.DeleteAsync(shopItem);

      return Ok(id);
    }
    catch (Exception exception)
    {
      return BadRequest(exception);
    }
  }
}
