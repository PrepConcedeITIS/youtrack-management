import React from 'react';
import PropTypes from 'prop-types';
import withTimerHOC from '@jetbrains/hub-widget-ui/dist/timer';

import './style/assignee-management-widget.css';
import {Button, Checkbox, H2, Text} from '@jetbrains/ring-ui';
import axios from 'axios';

class Content extends React.Component {

  static propTypes = {
    project: PropTypes.string
  };

  constructor(props) {
    super(props);

    this.state = {
      reTrainEnabled: false
    };

    this.setRetrainState = this.setRetrainState.bind(this);
  }

  // eslint-disable-next-line no-unused-vars
  componentDidUpdate(prevProps, prevState, snapshot) {
    if (prevProps && this.props && prevProps.project !== this.props.project) {
      this.getRetrainState().then(x => {
        this.setState({reTrainEnabled: x.data});
      });
    }
  }

  host() {
    return 'https://localhost:14000';
  }

  getRetrainState() {
    const promiseRes = axios.get(`${this.host()}/Ml/RetrainingState/${this.props.project}`);
    return promiseRes;
  }

  setRetrainState(state) {
    this.setState({reTrainEnabled: state});
  }

  renderWidgetBody() {
    const {
      project
    } = this.props;
    const host = this.host();
    const setRetrainState = this.setRetrainState;

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

    function changeRetrainState() {
      axios.post(`${host}/Ml/SwitchRetrainingState/${project}`).then(x => {
        setRetrainState(x.data);
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
        <div className={'row'}>
          <Text>{'Включить автоматическое переобучение: '}</Text>
          <Checkbox
            checked={this.state.reTrainEnabled}
            onChange={changeRetrainState}
          />
        </div>
      </div>
    );
  }

  render() {
    return this.renderWidgetBody();
  }
}


export default withTimerHOC(Content);
