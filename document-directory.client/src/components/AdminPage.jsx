import React, { Component } from 'react';
import { useEffect, useState } from 'react';
import { Divider, Menu, Switch } from 'antd';
import UserList from './UserList';
import { UserOutlined, TeamOutlined } from '@ant-design/icons';

import ListGroupComponent from './ListGroupComponent';

function AdminPage() {
    const [isShowListUser, setIsShowListUser] = useState(true);
    const [isShowListGroup, setIsShowListGroup] = useState(false);
    const navigate = useNavigate();

    async function checkAdmin() {
        const response = await fetch('https://localhost:7018/api/users/check-admin', {
            headers: new Headers({ "Content-Type": "application/json" }),
            credentials: 'include',
        })
        if (response.status == 403) {
            navigate("/")
        }
    }

    const handleChange = (item) => {

        if (item.key === '1') {
            setIsShowListUser(true);
            setIsShowListGroup(false);
        }
        else {
            setIsShowListUser(false);
            setIsShowListGroup(true);
        }
    }

    

    const menuItems = [
        {
            key: '1',
            icon: <UserOutlined />,
            label: 'Все пользователи',
        },
        {
            key: '2',
            icon: <TeamOutlined />,
            label: 'Все группы',
        }
    ]

    useEffect(() => {
        checkAdmin();
    }, [])

    return (
        <>
            <h3>Управление пользователями и группами</h3>
            <div style={{ display: 'flex' }}>
                <div>
                    <Menu id='menu'
                        style={{
                            width: 185,
                            textAlign: 'left',
                        }}
                        defaultSelectedKeys={['1']}
                        onClick={handleChange}
                        items={menuItems}

                    />
                </div>
                <div>
                    {isShowListUser && <UserList />}
                    {isShowListGroup && <ListGroupComponent />}
                      
                </div>
            </div>
            
        </>
    )
}
export default AdminPage;