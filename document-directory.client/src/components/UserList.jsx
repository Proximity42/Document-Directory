import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { message, Button, List, Select, Modal, Input, Form } from 'antd';
import { UserAddOutlined } from '@ant-design/icons';
import { EyeInvisibleOutlined, EyeTwoTone } from '@ant-design/icons';
import Cookie from 'js-cookie';

function UserList() {
    const [list, setList] = useState([]);
    const [isModalOpenDelete, setIsModalOpenDelete] = useState(false);
    const [isModalOpenChangePassword, setIsModalOpenChangePassword] = useState(false);
    const [isModalOpenCreate, setIsModalOpenCreate] = useState(false);
    const [valueRole, setValueRole] = useState(2)

    const [item, setItem] = useState({});
    const [roles, setRoles] = useState([]);
    const navigate = useNavigate();
    const [messageApi, contextHolder] = message.useMessage();

    

    const showModalDelete = (item) => {
        setItem(item)
        setIsModalOpenDelete(true);
    };

    const showModalChangePassword = (item) => {
        setItem(item)
        setIsModalOpenChangePassword(true);
    };

    const showModalCreate = () => {
        
        setIsModalOpenCreate(true);
        document.querySelector('#Login').value = '';
        document.querySelector('#Password').value = '';
        setValueRole(2);
    };

    async function handleOk() {
        const response = await fetch(`https://localhost:7018/api/users/${item.id}`, {
            method: 'DELETE',
            headers: new Headers({ "Content-Type": "application/json" }),
        });
        if (response.status == 200) {
            const newList = list.filter((itemList) => itemList.id !== item.id);
            setList(newList);
            messageApi.open({
                type: 'success',
                content: 'Пользователь удален',
            });
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
        setIsModalOpenDelete(false);
    };

    async function handleOkChange() {
        const response = await fetch('https://localhost:7018/api/users/password-change', {
            method: 'PATCH',
            headers: new Headers({ "Content-Type": "application/json" }),
            body: JSON.stringify({
                id: item.id,
                password: document.querySelector('#newPassword').value,
            })
        });
        if (response.status == 200) {
            messageApi.open({
                type: 'success',
                content: 'Пароль изменен',
            });
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
        setIsModalOpenChangePassword(false);
    };

    async function handleOkCreate() {
        const name = document.querySelector('#Login').value;
        const password = document.querySelector('#Password').value;
        const role = valueRole;
        const response = await fetch('https://localhost:7018/api/users', {
            method: 'POST',
            credentials: 'include',
            headers: new Headers({ "Content-Type": "application/json" }),
            body: JSON.stringify({
                roleId: role,
                login: name,
                password: password,
            })
        });
        if (response.status == 201) {
            const json = await response.json();
            setList([...list, json])
            messageApi.open({
                type: 'success',
                content: 'Пользователь добавлен',
            });
            
            setIsModalOpenCreate(false);
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
        
        
    }

    const handleCancel = () => {
        setIsModalOpenChangePassword(false);
        setIsModalOpenDelete(false);
        setIsModalOpenCreate(false);
    };

    function handleChangeCreate(value) {
        setValueRole(value)
    }

    async function handleChange(value, item) {
        setItem(item);
        item.roleId = value
        const response = await fetch('https://localhost:7018/api/users/rolechange', {
            method: 'PATCH',
            headers: new Headers({ "Content-Type": "application/json" }),
            body: JSON.stringify({
                id: item.id,
                roleId: item.roleId,
            })
        });
        if (response.status == 200) {
            messageApi.open({
                type: 'success',
                content: 'Роль пользователя изменена',
            });
        }
        else if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
    };

    async function getAllUsers() {
        const response = await fetch('https://localhost:7018/api/users/all', {
            method: 'GET',
            headers: new Headers({ "Content-Type": "application/json" }),
        })
        const json = await response.json();
        setList(json);
        if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
    }

    

    async function getRoles() {
        const response = await fetch('https://localhost:7018/api/role/all', {
            method: 'GET',
            headers: new Headers({ "Content-Type": "application/json" }),
        })
        const json = await response.json();
        setRoles(json);
        if (response.status == 401) {
            navigate('/login');
            Cookie.remove('test')
        }
    }

    

    useEffect(() => {
        getAllUsers();
        getRoles();
    }, []);
        
    

    return (
        <>
            {contextHolder}
            <List 
                style={{ width: 700 }}
                dataSource={list}
                header={<div style={{ textAlign: 'right', fontSize: 20 }}><a onClick={showModalCreate}><UserAddOutlined /> Добавить</a></div>}
                renderItem={(item) => (
                    <List.Item style={{ marginLeft: '10px', textAlign: 'left' }}
                        actions={[<Select style={{ width: '145px' }}
                            defaultValue={item.roleId}
                            onChange={(e) => handleChange(e, item)}
                            
                            options={[
                                {
                                    value: 1,
                                    label: "Администратор",
                                },
                                {
                                    value: 2,
                                    label: "Пользователь",
                                },
                            ]}
                        />, <a onClick={() => showModalChangePassword(item)}>Сменить пароль</a>, <a onClick={() => showModalDelete(item)}>Удалить</a>,]}
                    >{item.login} 

                    </List.Item>
                )}
            />
            <Modal title="Вы действительно хотите удалить пользователя?" open={isModalOpenDelete} onOk={handleOk} onCancel={handleCancel} footer={[
                <Button onClick={handleCancel}>Отменить</Button>, <Button onClick={handleOk}>Удалить</Button>
            ] }>
                <p>{ }</p>
                
            </Modal>
            <Modal title="Введите новый пароль" open={isModalOpenChangePassword} onOk={() => handleOkChange()} onCancel={() => handleCancel()} footer={[
                <Button onClick={() => handleCancel()}>Отменить</Button>, <Button onClick={() => handleOkChange(item)}>Изменить пароль</Button>
            ]}>
                <Input.Password id='newPassword' iconRender={(visible) => (visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />)} />
            </Modal>
            <Modal title="Создание пользователя" open={isModalOpenCreate} onOk={() => handleOkCreate()} onCancel={() => handleCancel()} footer={[
                <Button onClick={() => handleCancel()}>Отменить</Button>, <Button onClick={() => handleOkCreate()}>Создать</Button>
            ]}>
                <Form
                    
                    labelCol={{
                        span: 4,
                    }}
                >
                    <Form.Item
                        label="Логин"
                        name="login"
                        rules={[
                            {
                                required: true,
                                message: 'Введите логин нового пользователя',
                            },
                        ]}
                    >
                        <Input id='Login'/>
                    </Form.Item>
                    <Form.Item
                        label="Пароль"
                        name="password"
                        rules={[
                            {
                                required: true,
                                message: 'Введите пароль',
                            },
                        ]}>
                        <Input.Password id='Password' iconRender={(visible) => (visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />)} />
                    </Form.Item>
                    <Form.Item
                        label="Роль:"
                        name="role"
                        >
                        <Select style={{ width: '145px' }}
                            defaultValue={2}
                            onChange={handleChangeCreate}

                            options={[
                                {
                                    value: 1,
                                    label: "Администратор",
                                },
                                {
                                    value: 2,
                                    label: "Пользователь",
                                },
                            ]}
                        />
                    </Form.Item>
                </Form>
            </Modal>
        </>
    );
    
    
}
export default UserList;