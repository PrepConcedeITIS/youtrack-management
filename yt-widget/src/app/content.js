import React from 'react';
import PropTypes from 'prop-types';
import withTimerHOC from '@jetbrains/hub-widget-ui/dist/timer';

import './style/assignee-management-widget.css';
import {Button, H2} from '@jetbrains/ring-ui';
import axios from 'axios';

class Content extends React.Component {

  static propTypes = {
    project: PropTypes.string
  };

  constructor(props) {
    super(props);

    this.state = {
      expandedIssueId: null
    };
  }

  renderWidgetBody() {
    const {
      project
    } = this.props;
    const host = 'https://localhost:14000';

    function assignCurrentSprint() {
      return axios.post(`${host}/Sprint/AssignIssues`, {
        sprintName: 'Sprint 1',
        projectShortName: project
      });
    }

    function unAssignCurrentSprint() {
      return axios.post(`${host}/Sprint/DiscardAssignees`, {
        sprintName: 'Sprint 1',
        projectShortName: project
      });
    }

    function actualizeProjectAssignees() {
      return axios.post(`${host}/ManageAssignees/${project}`);
    }

    function reTrainModel() {
      return axios.post(`${host}/Ml/Train`, {
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
          onMouseDown={actualizeProjectAssignees}
        >{'Обновить команду разработки'}
        </Button>
        <H2>{'Управление обученной моделью'}</H2>
        <Button
          onMouseDown={reTrainModel}
        >{'Переобучить модель'}
        </Button>
      </div>
    );
  }

  render() {
    return this.renderWidgetBody();
  }
}


export default withTimerHOC(Content);
