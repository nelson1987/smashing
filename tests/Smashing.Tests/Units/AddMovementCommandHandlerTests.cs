using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smashing.Tests.Units
{
    internal class AddMovementCommandHandlerTests
    {
    }
    /*﻿using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentResults;
using FluentResults.Extensions.FluentAssertions;
using Moq;
using Treasury.Stock.CreditNotesManager.Core.Cross.Entities.CreditNotes;
using Treasury.Stock.CreditNotesManager.Core.Cross.Entities.SettlementOrders;
using Treasury.Stock.CreditNotesManager.Core.Features.PartialSettlement;
using Treasury.Stock.CreditNotesManager.Core.Features.PartialSettlement.Dtos;
using Treasury.Stock.CreditNotesManager.Core.Features.PartialSettlement.Events;
using Treasury.Stock.CreditNotesManager.Core.Features.PartialSettlement.PartialSettlementFlows;
using Treasury.Stock.CreditNotesManager.Core.Features.PartialSettlement.PartialSettlementFlows.CreditCardCreditNote;
using Treasury.Stock.CreditNotesManager.Tests.Factories;
using Xunit.Categories;

namespace Treasury.Stock.CreditNotesManager.Tests.Unit.Core.Features.PartialSettlement.PartialSettlementFlows.CreditCardCreditNote;

[UnitTest]
public class CreditCardCreditNotePartialSettlementFlowTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly Mock<ICreditCardCreditNotePartialSettlementFlowValidator> _creditCardCreditNotePartialSettlementFlowValidatorMock;
    private readonly Mock<IPartialSettlementOrderWriter> _partialSettlementOrderWriterMock;
    private readonly Mock<ISettlementEventsHandler> _settlementEventsHandlerMock;
    private readonly IPartialSettlementFlow _flow;

    private readonly CreditNote _creditNote;
    private readonly SettlementOrderEvent _settlementOrderEvent;

    public CreditCardCreditNotePartialSettlementFlowTests()
    {
        _creditNote = CreditNoteFactory.CreateCreditNoteClean();
        _settlementOrderEvent = _fixture.Build<SettlementOrderEvent>()
            .With(x => x.Asset, _fixture.Build<Asset>()
                .With(x => x.Name, AssetName.RevolvingCreditCard)
                .Create())
            .With(x => x.Instruction, OrderInstruction.PartialSettle)
            .Create();

        _fixture.Freeze<Mock<ISettlementOrderFactory>>()
            .Setup(x => x.Create(It.IsAny<SettlementOrderEvent>()))
            .Returns(Factories.SettlementOrderFactory.Create());

        _creditCardCreditNotePartialSettlementFlowValidatorMock = _fixture.Freeze<Mock<ICreditCardCreditNotePartialSettlementFlowValidator>>();
        _partialSettlementOrderWriterMock = _fixture.Freeze<Mock<IPartialSettlementOrderWriter>>();

        _settlementEventsHandlerMock = _fixture.Freeze<Mock<ISettlementEventsHandler>>();
        _flow = _fixture.Create<CreditCardCreditNotePartialSettlementFlow>();
    }

    #region CanHandle Tests

    [Theory]
    [InlineData(OrderInstruction.PartialSettle, true)]
    [InlineData(OrderInstruction.TotalSettle, false)]
    [InlineData(OrderInstruction.Undefined, false)]
    [InlineData(OrderInstruction.Issue, false)]
    [InlineData(OrderInstruction.Cancel, false)]
    [InlineData(OrderInstruction.Renegotiate, false)]
    [InlineData(OrderInstruction.Assign, false)]
    [InlineData(OrderInstruction.Endorse, false)]
    public void Given_a_settlement_order_event_when_validate_instruction_should_return_as_expected(OrderInstruction? instruction, bool expectedResult)
    {
        // Arrange
        var settlementOrderEvent = _settlementOrderEvent with { Instruction = instruction };

        // Act
        var result = _flow.CanHandle(settlementOrderEvent);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(AssetName.RevolvingCreditCard, true)]
    [InlineData(AssetName.RefinancingCreditCard, true)]
    [InlineData(AssetName.Loan, false)]
    [InlineData(AssetName.BankCreditNote, false)]
    [InlineData(AssetName.InstallmentCredit, false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void Given_a_settlement_order_event_when_validate_asset_name_should_return_as_expected(string? assetName,
        bool expectedResult)
    {
        // Arrange
        var settlementOrderEvent = _settlementOrderEvent with
        {
            Asset = _settlementOrderEvent.Asset! with { Name = assetName }
        };

        // Act
        var result = _flow.CanHandle(settlementOrderEvent);

        // Assert
        result.Should().Be(expectedResult);
    }

    #endregion

    #region Handle Tests

    [Fact]
    public async Task Given_valid_data_should_return_ok_result()
    {
        // Act
        var result = await _flow.Handle(_creditNote, _settlementOrderEvent, CancellationToken.None);

        // Assert
        result.Should().BeSuccess();
    }

    [Fact]
    public async Task Given_validation_error_should_repass_error_result()
    {
        // Arrange
        var failResult = Result.Fail("some error");
        _creditCardCreditNotePartialSettlementFlowValidatorMock
            .Setup(x => x.Validate(It.IsAny<CreditNote>(), It.IsAny<SettlementOrderEvent>()))
            .Returns(failResult);

        // Act
        var result = await _flow.Handle(_creditNote, _settlementOrderEvent, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(failResult);
    }

    [Fact]
    public async Task Given_total_settle_when_fails_on_credit_settle_should_return_error()
    {
        // Arrange
        var settlementOrderEvent = _settlementOrderEvent with { Instruction = OrderInstruction.TotalSettle };
        var creditNote = _creditNote with { Status = CreditNoteStatus.Cancelled };
        var expectedErrorMessage = $"The current state of the Credit Note is {creditNote.Status}, " +
                                   $"which prevents the requested action {Triggers.Settle} from being executed";

        // Act
        var result = await _flow.Handle(creditNote, settlementOrderEvent, CancellationToken.None);

        // Assert
        result.Should().BeFailure().And.HaveError(expectedErrorMessage);
    }

    [Fact]
    public async Task Given_save_error_should_repass_error_result()
    {
        // Arrange
        var failResult = Result.Fail("some error");
        _partialSettlementOrderWriterMock
            .Setup(x => x.Save(It.IsAny<SettlementOrder>(), CancellationToken.None))
            .ReturnsAsync(failResult);

        // Act
        var result = await _flow.Handle(_creditNote, _settlementOrderEvent, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(failResult);
    }

    [Fact]
    public async Task Given_settlement_handler_return_error_should_repass_error_result()
    {
        // Arrange
        var failResult = Result.Fail("some error");
        _settlementEventsHandlerMock
            .Setup(x => x.SendOrdersStateChanged(It.IsAny<SettlementOrder>()))
            .ReturnsAsync(failResult);

        // Act
        var result = await _flow.Handle(_creditNote, _settlementOrderEvent, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(failResult);
    }

    #endregion
}*/
}
