import React from 'react';
import PropTypes from 'prop-types';
import LoaderInline
from '@jetbrains/ring-ui/components/loader-inline/loader-inline';
import Link from '@jetbrains/ring-ui/components/link/link';
import {i18n} from 'hub-dashboard-addons/dist/localization';
import EmptyWidget, {
  EmptyWidgetFaces
} from '@jetbrains/hub-widget-ui/dist/empty-widget';
import withTimerHOC from '@jetbrains/hub-widget-ui/dist/timer';


import './style/issues-list-widget.css';
import {Button, H2} from '@jetbrains/ring-ui';
import axios from 'axios';

import IssueLine from './issue-line';

class Content extends React.Component {

  static propTypes = {
    youTrack: PropTypes.object,
    issues: PropTypes.array,
    issuesCount: PropTypes.number,
    isLoading: PropTypes.bool,
    fromCache: PropTypes.bool,
    isLoadDataError: PropTypes.bool,
    isNextPageLoading: PropTypes.bool,
    onLoadMore: PropTypes.func,
    onEdit: PropTypes.func,
    dateFormats: PropTypes.object,
    editable: PropTypes.bool,
    project: PropTypes.string
  };

  constructor(props) {
    super(props);

    this.state = {
      expandedIssueId: null
    };
  }

  renderNoIssuesError() {
    return (
      <EmptyWidget
        face={EmptyWidgetFaces.OK}
        message={i18n('No issues found')}
      >
        {
          this.props.editable &&
          (
            <Link
              pseudo
              onClick={this.props.onEdit}
            >
              {i18n('Edit search query')}
            </Link>
          )
        }
      </EmptyWidget>
    );
  }

  renderLoadDataError() {
    return (
      <EmptyWidget
        face={EmptyWidgetFaces.ERROR}
        message={i18n('Can\'t load information from service')}
      />
    );
  }

  getLoadMoreCount() {
    const {issuesCount, issues} = this.props;
    return (issues && issuesCount && issuesCount > issues.length)
      ? issuesCount - issues.length
      : 0;
  }

  renderLoader() {
    return <LoaderInline/>;
  }

  renderWidgetBody() {
    const {
      issues,
      youTrack,
      isNextPageLoading,
      dateFormats,
      project
    } = this.props;
    const {expandedIssueId} = this.state;
    const homeUrl = youTrack ? youTrack.homeUrl : '';
    const loadMoreCount = this.getLoadMoreCount();

    const setExpandedIssueId = issueId =>
      evt => {
        if (evt.target && evt.target.href) {
          return;
        }
        const isAlreadyExpanded = issueId === expandedIssueId;
        this.setState({expandedIssueId: isAlreadyExpanded ? null : issueId});
      };
    const host = 'https://localhost:14000/';

    function assignCurrentSprint() {
      return axios.post(`${host}/AssignSprint`, {
        sprintName: 'Sprint 1',
        projectShortName: project
      });
    }

    function unAssignCurrentSprint() {
      return axios.post(`${host}/unAssignSprint`, {
        sprintName: 'Sprint 1',
        projectShortName: project
      });
    }

    function actualizeProjectAssignees() {
      return axios.post(`${host}/ManageAssignees/${project}`);
    }

    function reTrainModel() {
      return axios.post(`${host}/Ml/train`, {
        projectShortName: project,
        withMock: true
      });
    }

    return (
      <div className="manage-assignees-widget">
        <H2>{'Управление спринтами'}</H2>
        <Button
          onMouseDown={assignCurrentSprint}
        >{'Распределить разработчиков на текущий спринт'}
        </Button>
        <Button
          className={'danger-button'}
          onMouseDown={unAssignCurrentSprint}
        >{'Сбросить распределение'}
        </Button>
        <H2>{'Управление списком разработчиков'}</H2>
        <Button
          onMouseDown={assignCurrentSprint}
        >{'Обновить команду разработки'}
        </Button>
        <H2>{'Управление обученной моделью'}</H2>
        <Button
          onMouseDown={assignCurrentSprint}
        >{'Переобучить модель'}
        </Button>
      </div>
    );
  }

  render() {
    const {
      issues,
      isLoading,
      fromCache,
      isLoadDataError
    } = this.props;

    if (!fromCache) {
      if (isLoadDataError) {
        return this.renderLoadDataError();
      }
      if (isLoading) {
        return this.renderLoader();
      }
    }
    if (!issues || !issues.length) {
      return this.renderNoIssuesError();
    }
    return this.renderWidgetBody();
  }
}


export default withTimerHOC(Content);
