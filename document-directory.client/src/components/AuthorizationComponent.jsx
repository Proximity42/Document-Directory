import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Input, Button } from 'antd';
import { UserOutlined } from '@ant-design/icons';
import { FolderFilled, FolderAddFilled, FileAddFilled, FileFilled, CloseOutlined, DeleteFilled } from '@ant-design/icons';

function Authorization() {
    const [user, setUser] = useState({});
    const navigate = useNavigate();

    async function authorization() {
        const login = document.querySelector('#inputLogin').value;
        const password = document.querySelector('#inputPassword').value;
        const response = await fetch('https://localhost:7018/api/authorization', {
            method: 'POST',
            headers: new Headers({ "Content-Type": "application/json" }),
            body: JSON.stringify({
                Login: login,
                Password: password
            })
        });
        if (response.status == 200) {
            const json = await response.json();
            setUser(json);
            navigate('/');
        }
        else if (response.status == 401) {
            document.getElementById('inputLogin').status = 'warning';
            document.getElementById('inputLogin').placeholder = "error";
            //#inputLogin.placeholder = 'Введите имя';
        }
    }

    return (
        <div style={{ margin: 'auto', marginTop: '15%' }}>
            <p style={{ fontSize: '26px' }}>Авторизация</p>    
            <div style={{margin: '10px'} }>
                <Input status="error" size='large' placeholder="Введите логин" id="inputLogin" prefix={<UserOutlined />} style={{ width: '25%', font: '16px' }} />
            </div>
            <div style={{ margin: '10px' }}>
                <Input size='large' placeholder="Введите пароль" id="inputPassword" style={{ width: '25%' }} />
            </div>
            <div>
                <Button type='primary' size='large' onClick={authorization} style={{ width: "130px" }}>Авторизация</Button>
            </div>            
        </div>
    )
}

export default Authorization;