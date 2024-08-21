import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Avatar, Button, List, Select, Modal } from 'antd';

function UserList() {
    const [list, setList] = useState([]);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [item, setItem] = useState({});
    const [roles, setRoles] = useState(getRoles())
    const navigate = useNavigate();
    
    

    const showModal = (item) => {
        setItem(item)
        setIsModalOpen(true);
    };
    const handleOk = () => {
        setIsModalOpen(false);
    };
    const handleCancel = () => {
        setIsModalOpen(false);
    };

    const handleChange = (value) => {
        console.log(`selected ${value}`);
    };

    async function getAllUsers() {
        const response = await fetch('https://localhost:7018/api/users/all', {
            method: 'GET',
            headers: new Headers({ "Content-Type": "application/json" }),
        })
        const json = await response.json();
        setList(json);
        getRoles()
    }

    

    async function getRoles() {
        const response = await fetch('https://localhost:7018/api/role/all', {
            method: 'GET',
            headers: new Headers({ "Content-Type": "application/json" }),
        })
        const json = await response.json();
        setRoles(json);
    }

    

    useEffect(() => {
        getAllUsers();
    }, []);
        
    

    return (
        <>
            <List 
                style={{ width: 700 }}
                dataSource={list}

                renderItem={(item) => (
                    <List.Item style={{ marginLeft: '10px', textAlign: 'left' }}
                        actions={[ <Select
                            defaultValue={item.roleId}
                            onChange={handleChange}
                            options={[
                                {
                                    value: roles[0].id,
                                    label: roles[0].userRole,
                                },
                                {
                                    value: roles[1].id,
                                    label: roles[1].userRole,
                                },

                            ]

                            }
                        />, <a onClick={() => console.log(item.id)}>Сменить пароль</a>, <a onClick={() => showModal(item)}>Удалить</a>,]}
                    >{item.login} 

                    </List.Item>
                )}
            />
            <Modal title="Вы действительно хотите удалить пользователя?" open={isModalOpen} onOk={handleOk} onCancel={handleCancel} footer={[
                <Button onClick={handleCancel}>Отменить</Button>, <Button onClick={handleOk}>Удалить</Button>
            ] }>
                <p>{ }</p>
                
            </Modal>
        </>
    );
    
    
}
export default UserList;