import './App.css';
import Button from "@jetbrains/ring-ui/components/button/button";
import DashboardAddons from "hub-dashboard-addons";
import {render} from "react-dom";

const App: React.FC = () => {
    const fetchApi = () => {
        fetch('https://localhost:17000/test')
            .then((response) => console.log(response));
    }

    return (
        <div className={'www'}>
            <Button onClick={fetchApi}>Click me</Button>
        </div>
    )
}

DashboardAddons.registerWidget((dashboardApi, registerWidgetApi) =>
    render(
        <Widget
            dashboardApi={dashboardApi}
            registerWidgetApi={registerWidgetApi}
        />,
        document.getElementById('app-container')
    )
);
