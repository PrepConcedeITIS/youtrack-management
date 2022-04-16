import React from 'react';
import PropTypes from 'prop-types';
import {i18n} from 'hub-dashboard-addons/dist/localization';
import ConfigurableWidget from '@jetbrains/hub-widget-ui/dist/configurable-widget';
import ServiceResources from '@jetbrains/hub-widget-ui/dist/service-resources';

import {
  loadDateFormats, loadIssues, loadTotalIssuesCount, ISSUES_PACK_SIZE
} from './resources';
import AssigneeManagementEditForm from './assignee-management-edit-form';

import './style/assignee-management-widget.css';
import Content from './content';

class AssigneeManagementWidget extends React.Component {

  static COUNTER_POLLING_PERIOD_SEC = 240; // eslint-disable-line no-magic-numbers
  static COUNTER_POLLING_PERIOD_MLS = 60000; // eslint-disable-line no-magic-numbers

  static getDefaultYouTrackService =
    async (dashboardApi, predefinedYouTrack) => {
      if (dashboardApi.loadServices) {
        return (await dashboardApi.loadServices('YouTrack')).
          filter(
            it => (predefinedYouTrack ? it.id === predefinedYouTrack.id : true)
          )[0];
      }

      try {
        return await ServiceResources.getYouTrackService(
          dashboardApi, predefinedYouTrack && predefinedYouTrack.id
        );
      } catch (err) {
        return null;
      }
    };

  static youTrackServiceNeedsUpdate = service => !service.name;

  static getDefaultWidgetTitle = () =>
    i18n('Панель управления автоматическим распределением задач');

  static getWidgetProject = context => {
    if (context && context.shortName) {
      return context.shortName;
    } else {
      return '';
    }
  };

  static getWidgetTitle = (search, context) => {
    if (context && context.name) {
      return `${context.name}. ${AssigneeManagementWidget.getDefaultWidgetTitle()}`;
    } else {
      return AssigneeManagementWidget.getDefaultWidgetTitle();
    }
  };

  static propTypes = {
    dashboardApi: PropTypes.object,
    configWrapper: PropTypes.object,
    registerWidgetApi: PropTypes.func,
    editable: PropTypes.bool
  };

  constructor(props) {
    super(props);
    const {registerWidgetApi} = props;

    this.state = {
      isConfiguring: false,
      isLoading: true
    };

    registerWidgetApi({
      onConfigure: () => this.setState({
        isConfiguring: true,
        isLoading: false,
        isLoadDataError: false
      }),
      onRefresh: () => this.load(),
      getExternalWidgetOptions: () => ({
        authClientId:
          (this.props.configWrapper.getFieldValue('youTrack') || {}).id
      })
    });
  }

  componentDidMount() {
    this.initialize(this.props.dashboardApi);
  }

  initialize = async dashboardApi => {
    this.setState({isLoading: true});
    await this.props.configWrapper.init();

    const youTrackService =
      await AssigneeManagementWidget.getDefaultYouTrackService(
        dashboardApi, this.props.configWrapper.getFieldValue('youTrack')
      );

    if (this.props.configWrapper.isNewConfig()) {
      this.initializeNewWidget(youTrackService);
    } else {
      await this.initializeExistingWidget(youTrackService);
    }
  };

  initializeNewWidget(youTrackService) {
    if (youTrackService && youTrackService.id) {
      this.setState({
        isConfiguring: true,
        youTrack: youTrackService,
        isLoading: false
      });
    }
    this.setState({isLoadDataError: true, isLoading: false});
  }

  async initializeExistingWidget(youTrackService) {
    const search = this.props.configWrapper.getFieldValue('search');
    const context = this.props.configWrapper.getFieldValue('context');
    const refreshPeriod =
      this.props.configWrapper.getFieldValue('refreshPeriod');
    const title = this.props.configWrapper.getFieldValue('title');

    this.setState({
      title,
      search: search || '',
      context,
      refreshPeriod:
        refreshPeriod || AssigneeManagementWidget.COUNTER_POLLING_PERIOD_SEC
    });

    if (youTrackService && youTrackService.id) {
      const onYouTrackSpecified = async () => {
        await this.load(search, context);
        const dateFormats = await loadDateFormats(
          this.fetchYouTrack
        );
        this.setState({dateFormats, isLoading: false});
      };
      this.setYouTrack(youTrackService, onYouTrackSpecified);
    }
  }

  setYouTrack(youTrackService, onAfterYouTrackSetFunction) {
    const {homeUrl} = youTrackService;

    this.setState({
      youTrack: {
        id: youTrackService.id, homeUrl
      }
    }, async () => await onAfterYouTrackSetFunction());

    if (AssigneeManagementWidget.youTrackServiceNeedsUpdate(youTrackService)) {
      const {dashboardApi} = this.props;
      ServiceResources.getYouTrackService(
        dashboardApi, youTrackService.id
      ).then(
        updatedYouTrackService => {
          const shouldReSetYouTrack = updatedYouTrackService &&
            !AssigneeManagementWidget.youTrackServiceNeedsUpdate(
              updatedYouTrackService
            ) && updatedYouTrackService.homeUrl !== homeUrl;
          if (shouldReSetYouTrack) {
            this.setYouTrack(
              updatedYouTrackService, onAfterYouTrackSetFunction
            );
            if (!this.state.isConfiguring && this.props.editable) {
              this.props.configWrapper.update({
                youTrack: {
                  id: updatedYouTrackService.id,
                  homeUrl: updatedYouTrackService.homeUrl
                }
              });
            }
          }
        }
      );
    }
  }

  submitConfiguration = async formParameters => {
    const {
      search, title, context, refreshPeriod, selectedYouTrack
    } = formParameters;
    this.setYouTrack(
      selectedYouTrack, async () => {
        this.setState(
          {search: search || '', context, title, refreshPeriod},
          async () => {
            await this.load();
            await this.props.configWrapper.replace({
              search,
              context,
              title,
              refreshPeriod,
              youTrack: {
                id: selectedYouTrack.id,
                homeUrl: selectedYouTrack.homeUrl
              }
            });
            this.setState(
              {isConfiguring: false, fromCache: false}
            );
          }
        );
      }
    );
  };

  cancelConfiguration = async () => {
    if (this.props.configWrapper.isNewConfig()) {
      await this.props.dashboardApi.removeWidget();
    } else {
      this.setState({isConfiguring: false});
      await this.props.dashboardApi.exitConfigMode();
      this.initialize(this.props.dashboardApi);
    }
  };

  fetchYouTrack = async (url, params) => {
    const {dashboardApi} = this.props;
    const {youTrack} = this.state;
    return await dashboardApi.fetch(youTrack.id, url, params);
  };

  renderConfiguration = () => (
    <div className="assignee-management-widget">
      <AssigneeManagementEditForm
        search={this.state.search}
        context={this.state.context}
        title={this.state.title}
        refreshPeriod={this.state.refreshPeriod}
        onSubmit={this.submitConfiguration}
        onCancel={this.cancelConfiguration}
        dashboardApi={this.props.dashboardApi}
        youTrackId={this.state.youTrack.id}
      />
    </div>
  );

  load = async () => {
    this.setState({isLoadDataError: false});
  };

  renderContent = () => {
    const {
      context
    } = this.state;
    return (
      <Content
        project={AssigneeManagementWidget.getWidgetProject(context)}
      />
    );
  };

  // eslint-disable-next-line complexity
  render() {
    const {
      isConfiguring,
      search,
      context
    } = this.state;

    const widgetTitle = isConfiguring
      ? AssigneeManagementWidget.getDefaultWidgetTitle()
      : AssigneeManagementWidget.getWidgetTitle(
        search, context
      );

    return (
      <ConfigurableWidget
        isConfiguring={isConfiguring}
        dashboardApi={this.props.dashboardApi}
        widgetTitle={widgetTitle}
        widgetLoader={this.state.isLoading}
        Configuration={this.renderConfiguration}
        Content={this.renderContent}
      />
    );
  }
}


export default AssigneeManagementWidget;
