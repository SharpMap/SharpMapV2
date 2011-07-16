using System;
using System.Collections;
using GeoAPI.Geometries;
using NetTopologySuite.CoordinateSystems;

using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using SharpMap.Presentation;
using SharpMap.Presentation.Presenters;
using SharpMap.Presentation.Views;
using Xunit;

namespace SharpMap.Tests.Presentation
{

    public class ToolsPresenterTests : IUseFixture<FixtureFactories>
    {
        private FixtureFactories _factories;

        public void SetFixture(FixtureFactories data)
        {
            _factories = data;
        }

        [Fact]
        public void PresenterSetsTools()
        {
            MockRepository mocks = new MockRepository();

            IToolsView view = mocks.Stub<IToolsView>();

            mocks.ReplayAll();

            Map map = new Map(_factories.GeoFactory);
            ToolsPresenter toolsPresenter = new ToolsPresenter(map, view);

            Assert.NotNull(view.Tools);
            Assert.NotEqual(0, view.Tools.Count);
        }

        //[Fact]
        //public void ViewInitiatesToolSelectionChangeRequest()
        //{
        //    MockRepository mocks = new MockRepository();

        //    IToolsView view = mocks.Stub<IToolsView>();

        //    view.ToolChangeRequested += null;
        //    IEventRaiser toolChangeRequest = LastCall.IgnoreArguments().GetEventRaiser();

        //    mocks.ReplayAll();

        //    Map map = new Map(_geoFactory);
        //    ToolsPresenter toolsPresenter = new ToolsPresenter(map, view);

        //    Random rnd = new Random();
        //    Int32 toolIndex = rnd.Next(0, view.Tools.TotalItemCount - 1);

        //    ToolChangeRequestedEventArgs requestArgs = new ToolChangeRequestedEventArgs(view.Tools[toolIndex]);
        //    toolChangeRequest.Raise(view, requestArgs);

        //    Assert.Same(view.SelectedTool, view.Tools[toolIndex]);
        //}
    }
}
