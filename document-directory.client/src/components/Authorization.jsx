import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Cookies from 'universal-cookie';
//import Cookies from 'js-cookie';
import { Input, Button, Form, message } from 'antd';
import { EyeInvisibleOutlined, EyeTwoTone } from '@ant-design/icons';
import { LockOutlined, UserOutlined } from '@ant-design/icons';


function Authorization() {
    const [user, setUser] = useState({});
    const navigate = useNavigate();

    const [messageApi, contextHolder] = message.useMessage();
        
    async function authorization() {
        const login = document.querySelector('#inputLogin').value;
        const password = document.querySelector('#inputPassword').value;
        const response = await fetch('https://localhost:7018/api/authorization', {
            method: 'POST',
            headers: new Headers({ "Content-Type": "application/json" }),
            credentials: 'include',
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
            
            messageApi.open({
                type: 'error',
                content: 'Логин или пароль введены неверно',
            });
        }
    }

    return (
        <>
            <div style={{ marginTop: '15%' }}>
                <h3>Авторизация</h3>
                {contextHolder }
                <Form name='auth' onFinish={authorization}>
                    <Form.Item
                        name="login"
                        rules={[
                            {
                                required: true,
                                message: 'Введите логин',
                            },

                        ]}
                    >
                        <Input size="large" style={{ width: '25%' }} prefix={<UserOutlined />} placeholder="Логин" id='inputLogin' />

                    </Form.Item>
                    <Form.Item
                        name='password'
                        rules={[
                            {
                                required: true,
                                message: 'Введите пароль',
                            }
                        ]}
                    >
                        <Input.Password size="large" style={{ width: '25%' }} prefix={<LockOutlined />} placeholder="Пароль" id='inputPassword' iconRender={(visible) => (visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />)} />

                    </Form.Item>
                    <Form.Item>
                        <Button type='primary' htmlType="submit">Авторизоваться</Button>
                    </Form.Item>
                </Form>
            </div>
        
        </>
    )
}
export default Authorization;