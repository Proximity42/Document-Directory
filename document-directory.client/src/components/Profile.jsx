import { useEffect, useState, Suspense } from 'react';
import { List, Modal, Button, Form, Input, message } from 'antd';
import { EyeInvisibleOutlined, EyeTwoTone } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import Cookie from 'js-cookie';
import { isJwtExpired } from 'jwt-check-expiration';


function Profile() {
    const [user, setUser] = useState({ login: '', role: { id: '', userRole: '' } });
    const [groups, setGroups] = useState([]);
    const [selectGroup, setSelectGroup] = useState({});
    const [participants, setParticipants] = useState([]);
    const [isShowModalParticipants, setIsShowModalParticipants] = useState(false);
    const [isShowModalPassword, setIsShowModalPassword] = useState(false);
    const [isShowModalExit, setIsShowModalExit] = useState(false);
    const navigate = useNavigate();

    const [messageApi, contextHolder] = message.useMessage();
    async function getUser() {
        if (isJwtExpired(Cookie.get('test'))) {
            navigate("/login");
            Cookie.remove('test');
            return;
        }
        const response = await fetch('https://localhost:7018/api/users/current-user', {
            method: 'GET',
            credentials: 'include',
            headers: new Headers({ "Content-Type": "application/json" }),
        })
        if (response.status == 200) {
            const json = await response.json();
            setUser(json);
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
    }

    function showModalParticipants(group) {
        setSelectGroup(group);
        getParticipantsGroup(group.id)
        setIsShowModalParticipants(true);
    }

    function showModalPassword() {
        
        setIsShowModalPassword(true);
        document.querySelector('#Password').value = '';
        this.setState({value: '' })
    }
    function showModalExit() {
        setIsShowModalExit(true);
    }


    async function getGroups() {
        if (isJwtExpired(Cookie.get('test'))) {
            navigate("/login");
            Cookie.remove('test');
            return;
        }
        const response = await fetch('https://localhost:7018/api/groups/user', {
            method: 'GET',
            credentials: 'include',
            headers: new Headers({ "Content-Type": "application/json" }),
        })
        if (response.status == 200) {
            const json = await response.json();
            setGroups(json);
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
    }

    async function getParticipantsGroup(id) {
        if (isJwtExpired(Cookie.get('test'))) {
            navigate("/login");
            Cookie.remove('test');
            return;
        }
        const response = await fetch(`https://localhost:7018/api/groups/${id}`, {
            method: 'GET',
            credentials: 'include',
            headers: new Headers({ "Content-Type": "application/json" }),
        })
        if (response.status == 200) {
            const json = await response.json();
            setParticipants(json)
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
    }

    const handleCancel = () => {
        
        
        setIsShowModalParticipants(false);
        setIsShowModalPassword(false);
        setIsShowModalExit(false);
        
        
    }
    const handleOk = () => {

    }
    async function changePassword() {
        const currentPassword = document.querySelector('#Password').value;
        const newPassword = document.querySelector('#newPassword').value;
        const confirmPassword = document.querySelector('#confirmPassword').value;

        if (newPassword != confirmPassword) {
            document.querySelector('#Password').value = '';
            document.querySelector('#newPassword').value = '';
            document.querySelector('#confirmPassword').value = '';

            messageApi.open({
                type: 'error',
                content: "Пароли не совпадают",
            })
        }
        else {
            if (isJwtExpired(Cookie.get('test'))) {
                navigate("/login");
                Cookie.remove('test');
                return;
            }
            const response = await fetch('https://localhost:7018/api/users/password-change-authorized', {
                method: 'PATCH',
                credentials: 'include',
                headers: new Headers({ "Content-Type": "application/json" }),
                body: JSON.stringify({
                    oldPassword: currentPassword,
                    newPassword: newPassword
                })
            })
            if (response.status == 400) {
                document.querySelector('#Password').value = '';
                document.querySelector('#newPassword').value = '';
                document.querySelector('#confirmPassword').value = '';

                messageApi.open({
                    type: 'error',
                    content: "Пароль введен неверно",
                })
            }
            else if (response.status == 401) {
                navigate('/login');
                Cookie.remove('test')
            }
            else if (response.status == 200) {
                messageApi.open({
                    type: 'success',
                    content: 'Пароль изменен'
                })
                setIsShowModalPassword(false)
            }
        }
    }
    async function exit() {
        setIsShowModalExit(true);
        navigate('/login');
        Cookie.remove('test')

    }

    
    useEffect(() => {
        getUser();
        getGroups()
    }, [])
    

    return (
        <>
            {contextHolder}
            <h4>Ваш профиль</h4>
            <p> {user.login} - {user.role.userRole}</p>
            <p>Ваши группы:</p>
            <List
                // footer={[<Button onClick={showModalPassword}>Сменить пароль</Button>, <Button onClick={showModalExit}>Выйти из профиля</Button>]}
                dataSource={groups}
                renderItem={(item) => (
                    <List.Item
                        actions={[<a onClick={() => showModalParticipants(item)}>Участники</a>]}>{item.name}
                    </List.Item>

                )}
                style={{width: '80%', margin: 'auto'}}
            />
            <div style={{display: 'flex', gap: '10px', justifyContent: 'center'}}>
                <Button onClick={showModalPassword}>Сменить пароль</Button>
                <Button onClick={showModalExit}>Выйти из профиля</Button>
            </div>
            <Modal title="Участники группы:" open={isShowModalParticipants} onCancel={handleCancel} footer={[
                <Button onClick={handleCancel}>Назад</Button>
            ]}>
                <List
                    dataSource={participants}
                    renderItem={(item) => (
                        <List.Item>
                            {item.login}
                        </List.Item>
                    )}
                />
            </Modal>
            <Modal title="Смена пароля" open={isShowModalPassword} onCancel={handleCancel} footer={[
                <Button onClick={handleCancel}>Отменить</Button>, <Button onClick={changePassword }>Сменить</Button>
            ]}>
                <Form>
                    <Form.Item
                        label="Текущий пароль"
                        name="currentPassword"
                        rules={[
                            {
                                required: true,
                                message: 'Введите текущий пароль'
                            },
                        ]}
                    >
                        <Input.Password id='Password' iconRender={(visible) => (visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />)} />
                    </Form.Item>

                    <Form.Item
                        label="Новый пароль"
                        name="newPassword"
                        rules={[
                            {
                                required: true,
                                message: 'Введите новый пароль'
                            }
                        ]}
                    >
                        <Input.Password id='newPassword' iconRender={(visible) => (visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />)} />
                    </Form.Item>

                    <Form.Item
                        label="Подтвердите новый пароль"
                        name="confirmPassword"
                        dependencies={['newPassword']}
                        rules={[
                            {
                                required: true,
                                message: 'Подтвердите пароль'
                            },
                            ({ getFieldValue }) => ({
                                validator(_, value) {
                                    if (!value || getFieldValue('newPassword') === value) {
                                        return Promise.resolve();
                                    }
                                    return Promise.reject(new Error('Пароли не соответствуют'));
                                },
                            }),
                        ]}
                    > 
                        <Input.Password id='confirmPassword' iconRender={(visible) => (visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />)} />
                    </Form.Item>
                </Form>
            </Modal>
            <Modal title="Вы точно хотите выйти?" open={isShowModalExit} onCancel={handleCancel} footer={[
                <Button onClick={handleCancel}>Отменить</Button>, <Button onClick={exit}>Выйти</Button>
            ]}>
            </Modal>
        </>
    )
}
export default Profile